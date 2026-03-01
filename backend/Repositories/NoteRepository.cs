using Dapper;
using Npgsql;
using NotesApp.Api.Models;

namespace NotesApp.Api.Repositories;

public interface INoteRepository
{
    Task<IEnumerable<Note>> GetAllAsync(int userId);
    Task<Note?> GetByIdAsync(int id, int userId);
    Task<int> CreateAsync(Note note);
    Task<bool> UpdateAsync(Note note);
    Task<bool> DeleteAsync(int id, int userId);
}

public class NoteRepository : INoteRepository
{
    private readonly NpgsqlDataSource _dataSource;

    public NoteRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<IEnumerable<Note>> GetAllAsync(int userId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        const string sql = @"SELECT * FROM ""Notes"" WHERE ""UserId"" = @UserId ORDER BY ""CreatedAt"" DESC";
        return await connection.QueryAsync<Note>(sql, new { UserId = userId });
    }

    public async Task<Note?> GetByIdAsync(int id, int userId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        const string sql = @"SELECT * FROM ""Notes"" WHERE ""Id"" = @Id AND ""UserId"" = @UserId";
        return await connection.QueryFirstOrDefaultAsync<Note>(sql, new { Id = id, UserId = userId });
    }

    public async Task<int> CreateAsync(Note note)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        const string sql = @"
            INSERT INTO ""Notes"" (""Title"", ""Content"", ""CreatedAt"", ""UpdatedAt"", ""UserId"")
            VALUES (@Title, @Content, @CreatedAt, @UpdatedAt, @UserId)
            RETURNING ""Id"";";
        return await connection.ExecuteScalarAsync<int>(sql, note);
    }

    public async Task<bool> UpdateAsync(Note note)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        const string sql = @"
            UPDATE ""Notes""
            SET ""Title"" = @Title, ""Content"" = @Content, ""UpdatedAt"" = @UpdatedAt
            WHERE ""Id"" = @Id AND ""UserId"" = @UserId";
        var rowsAffected = await connection.ExecuteAsync(sql, note);
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        await using var connection = await _dataSource.OpenConnectionAsync();
        const string sql = @"DELETE FROM ""Notes"" WHERE ""Id"" = @Id AND ""UserId"" = @UserId";
        var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id, UserId = userId });
        return rowsAffected > 0;
    }
}
