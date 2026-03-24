using CarMarketplace.Api.Common;
using CarMarketplace.Api.Dtos;

namespace CarMarketplace.Api.Services;

public interface ICarService
{
    Task<PagedResponse<CarListingResponseDto>> SearchPublicAsync(CarSearchRequestDto request, CancellationToken cancellationToken);

    Task<CarListingResponseDto> GetPublicByIdAsync(int id, CancellationToken cancellationToken);

    Task<CarListingResponseDto> CreateAsync(int sellerId, CreateCarRequestDto request, CancellationToken cancellationToken);

    Task<CarListingResponseDto> UpdateAsync(int carId, int currentUserId, string currentUserRole, UpdateCarRequestDto request, CancellationToken cancellationToken);

    Task DeleteAsync(int carId, int currentUserId, string currentUserRole, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CarListingResponseDto>> GetMineAsync(int sellerId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<CarListingResponseDto>> GetAllForAdminAsync(CancellationToken cancellationToken);

    Task SetLockStateAsync(int carId, bool isLocked, CancellationToken cancellationToken);
}
