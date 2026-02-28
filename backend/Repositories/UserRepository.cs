using Dapper;
using Microsoft.Data.SqlClient;
using NotesApp.Api.Models;

namespace NotesApp.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<int> CreateAsync(User user);
}

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException("Connection string not found");
    }

    private SqlConnection CreateConnection() => new(_connectionString);

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = CreateConnection();
        const string sql = "SELECT * FROM Users WHERE Username = @Username";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = CreateConnection();
        const string sql = @"
            INSERT INTO Users (Username, PasswordHash)
            VALUES (@Username, @PasswordHash);
            SELECT CAST(SCOPE_IDENTITY() as int);";
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }
}
