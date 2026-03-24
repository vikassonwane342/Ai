using CarMarketplace.Api.Dtos;

namespace CarMarketplace.Api.Services;

public interface IAdminService
{
    Task<IReadOnlyCollection<UserResponseDto>> GetUsersAsync(AdminUserFilterDto filter, CancellationToken cancellationToken);

    Task SetUserLockStateAsync(int userId, bool isLocked, CancellationToken cancellationToken);
}
