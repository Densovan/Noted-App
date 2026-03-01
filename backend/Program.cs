using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NotesApp.Api.Repositories;
using NotesApp.Api.Services;
using Npgsql;
using Dapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotesApp API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// DI
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Register NpgsqlDataSource (handles both URL and key-value connection strings + SSL)
var rawConnString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("Connection string not found");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(rawConnString);
dataSourceBuilder.ConnectionStringBuilder.SslMode = SslMode.Prefer;
var dataSource = dataSourceBuilder.Build();
builder.Services.AddSingleton(dataSource);

// Auth
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("JWT Key not found");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Swagger always enabled (needed on Render too)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize Database (PostgreSQL)
using (var scope = app.Services.CreateScope())
{
    var ds = scope.ServiceProvider.GetRequiredService<NpgsqlDataSource>();
    var sqlPath = Path.Combine(app.Environment.ContentRootPath, "Data", "init.sql");

    if (!File.Exists(sqlPath))
        sqlPath = Path.Combine(AppContext.BaseDirectory, "Data", "init.sql");

    if (File.Exists(sqlPath))
    {
        Console.WriteLine($"Found init.sql at {sqlPath}. Starting initialization...");
        try
        {
            await using var conn = await ds.OpenConnectionAsync();
            var sql = await File.ReadAllTextAsync(sqlPath);
            await conn.ExecuteAsync(sql);
            Console.WriteLine("Database tables initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine($"Warning: init.sql not found at {sqlPath}. Skipping.");
    }
}

app.Run();
