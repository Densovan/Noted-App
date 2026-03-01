using Dapper;
using Npgsql;
using NotesApp.Api.Models;

namespace NotesApp.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<int> CreateAsync(User user);
}

public class UserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public UserRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        const string sql = @"SELECT * FROM ""Users"" WHERE ""Username"" = @Username";
        return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
    }

    public async Task<int> CreateAsync(User user)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        const string sql = @"
            INSERT INTO ""Users"" (""Username"", ""PasswordHash"")
            VALUES (@Username, @PasswordHash)
            RETURNING ""Id"";";
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }
}
