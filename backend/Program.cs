using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NotesApp.Api.Repositories;
using NotesApp.Api.Services;

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize Database
using (var scope = app.Services.CreateScope())
{
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var connString = config.GetConnectionString("DefaultConnection");
    var sqlPath = Path.Combine(app.Environment.ContentRootPath, "Data", "init.sql");
    
    // Check fallback path (for published builds)
    if (!File.Exists(sqlPath))
    {
        sqlPath = Path.Combine(AppContext.BaseDirectory, "Data", "init.sql");
    }

    if (File.Exists(sqlPath))
    {
        Console.WriteLine($"Found init.sql at {sqlPath}. Starting initialization...");
        try 
        {
            var sqlBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connString);
            var targetDatabase = sqlBuilder.InitialCatalog;
            sqlBuilder.InitialCatalog = "master"; 
            
            using var conn = new Microsoft.Data.SqlClient.SqlConnection(sqlBuilder.ConnectionString);
            await conn.OpenAsync();
            
            // 1. Create database if it doesn't exist
            await Dapper.SqlMapper.ExecuteAsync(conn, $@"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{targetDatabase}')
                BEGIN
                    CREATE DATABASE [{targetDatabase}];
                END");
            
            // 2. Switch to the target database
            await Dapper.SqlMapper.ExecuteAsync(conn, $"USE [{targetDatabase}];");
            
            // 3. Run the rest of the script
            var sql = File.ReadAllText(sqlPath);
            var batches = sql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var batch in batches)
            {
                if (!string.IsNullOrWhiteSpace(batch) && !batch.Contains("CREATE DATABASE", StringComparison.OrdinalIgnoreCase))
                {
                    await Dapper.SqlMapper.ExecuteAsync(conn, batch);
                }
            }
            Console.WriteLine($"Database '{targetDatabase}' and tables initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
    }
    else 
    {
        Console.WriteLine($"Warning: init.sql not found at {sqlPath}. Skipping database initialization.");
    }
}

app.Run();
