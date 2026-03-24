using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Entities;

namespace CarMarketplace.Api.Repositories;

public interface ICarRepository
{
    Task<(IReadOnlyCollection<CarListing> Items, int TotalCount)> SearchPublicAsync(CarSearchRequestDto request, CancellationToken cancellationToken);

    Task<CarListing?> GetPublicByIdAsync(int id, CancellationToken cancellationToken);

    Task<CarListing?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<int> CreateAsync(CarListing carListing, CancellationToken cancellationToken);

    Task UpdateAsync(CarListing carListing, CancellationToken cancellationToken);

    Task DeleteAsync(int id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CarListing>> GetBySellerAsync(int sellerId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CarListing>> GetAllAsync(CancellationToken cancellationToken);

    Task SetLockedAsync(int id, bool isLocked, CancellationToken cancellationToken);
}
