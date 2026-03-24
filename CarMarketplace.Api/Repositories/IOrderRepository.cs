using CarMarketplace.Api.Entities;

namespace CarMarketplace.Api.Repositories;

public interface IOrderRepository
{
    Task<int> CreateAsync(Order order, CancellationToken cancellationToken);

    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Order>> GetByBuyerIdAsync(int buyerId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Order>> GetBySellerIdAsync(int sellerId, CancellationToken cancellationToken);

    Task UpdateStatusAsync(int id, string status, string? notes, CancellationToken cancellationToken);
}
