using System.Data;
using CarMarketplace.Api.Entities;
using Dapper;

namespace CarMarketplace.Api.Repositories;

public class UserRepository(IDbConnection dbConnection) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, FirstName, LastName, Email, Phone, PasswordHash, Role, IsLocked, CreatedAt, UpdatedAt
            FROM Users
            WHERE Email = @Email;
            """;

        return await dbConnection.QuerySingleOrDefaultAsync<User>(new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken));
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, FirstName, LastName, Email, Phone, PasswordHash, Role, IsLocked, CreatedAt, UpdatedAt
            FROM Users
            WHERE Id = @Id;
            """;

        return await dbConnection.QuerySingleOrDefaultAsync<User>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<int> CreateAsync(User user, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO Users (FirstName, LastName, Email, Phone, PasswordHash, Role, IsLocked, CreatedAt, UpdatedAt)
            VALUES (@FirstName, @LastName, @Email, @Phone, @PasswordHash, @Role, @IsLocked, SYSUTCDATETIME(), SYSUTCDATETIME());
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await dbConnection.ExecuteScalarAsync<int>(new CommandDefinition(sql, user, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<User>> GetUsersAsync(string? role, bool? isLocked, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, FirstName, LastName, Email, Phone, PasswordHash, Role, IsLocked, CreatedAt, UpdatedAt
            FROM Users
            WHERE (@Role IS NULL OR Role = @Role)
              AND (@IsLocked IS NULL OR IsLocked = @IsLocked)
            ORDER BY CreatedAt DESC;
            """;

        var users = await dbConnection.QueryAsync<User>(new CommandDefinition(sql, new { Role = role, IsLocked = isLocked }, cancellationToken: cancellationToken));
        return users.ToArray();
    }

    public async Task SetLockedAsync(int id, bool isLocked, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE Users
            SET IsLocked = @IsLocked,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id;
            """;

        await dbConnection.ExecuteAsync(new CommandDefinition(sql, new { Id = id, IsLocked = isLocked }, cancellationToken: cancellationToken));
    }
}
