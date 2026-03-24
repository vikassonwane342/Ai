using System.Data;
using CarMarketplace.Api.Entities;
using Dapper;

namespace CarMarketplace.Api.Repositories;

public class OrderRepository(IDbConnection dbConnection) : IOrderRepository
{
    public async Task<int> CreateAsync(Order order, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO Orders (CarListingId, BuyerId, SellerId, Status, Notes, BookedAt, UpdatedAt)
            VALUES (@CarListingId, @BuyerId, @SellerId, @Status, @Notes, SYSUTCDATETIME(), SYSUTCDATETIME());
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        return await dbConnection.ExecuteScalarAsync<int>(new CommandDefinition(sql, order, cancellationToken: cancellationToken));
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var sql = GetBaseQuery() + " WHERE o.Id = @Id;";
        return await dbConnection.QuerySingleOrDefaultAsync<Order>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<Order>> GetByBuyerIdAsync(int buyerId, CancellationToken cancellationToken)
    {
        var sql = GetBaseQuery() + " WHERE o.BuyerId = @BuyerId ORDER BY o.BookedAt DESC;";
        var orders = await dbConnection.QueryAsync<Order>(new CommandDefinition(sql, new { BuyerId = buyerId }, cancellationToken: cancellationToken));
        return orders.ToArray();
    }

    public async Task<IReadOnlyCollection<Order>> GetBySellerIdAsync(int sellerId, CancellationToken cancellationToken)
    {
        var sql = GetBaseQuery() + " WHERE o.SellerId = @SellerId ORDER BY o.BookedAt DESC;";
        var orders = await dbConnection.QueryAsync<Order>(new CommandDefinition(sql, new { SellerId = sellerId }, cancellationToken: cancellationToken));
        return orders.ToArray();
    }

    public async Task UpdateStatusAsync(int id, string status, string? notes, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE Orders
            SET Status = @Status,
                Notes = @Notes,
                UpdatedAt = SYSUTCDATETIME()
            WHERE Id = @Id;
            """;

        await dbConnection.ExecuteAsync(new CommandDefinition(sql, new { Id = id, Status = status, Notes = notes }, cancellationToken: cancellationToken));
    }

    private static string GetBaseQuery() => """
        SELECT o.Id, o.CarListingId, o.BuyerId, o.SellerId, o.Status, o.Notes, o.BookedAt, o.UpdatedAt,
               CONCAT(b.FirstName, ' ', b.LastName) AS BuyerName,
               CONCAT(s.FirstName, ' ', s.LastName) AS SellerName,
               cl.Title AS CarTitle
        FROM Orders o
        INNER JOIN Users b ON b.Id = o.BuyerId
        INNER JOIN Users s ON s.Id = o.SellerId
        INNER JOIN CarListings cl ON cl.Id = o.CarListingId
        """;
}
