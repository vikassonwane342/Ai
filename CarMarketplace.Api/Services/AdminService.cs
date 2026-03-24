using CarMarketplace.Api.Common;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Repositories;

namespace CarMarketplace.Api.Services;

public class AdminService(IUserRepository userRepository) : IAdminService
{
    public async Task<IReadOnlyCollection<UserResponseDto>> GetUsersAsync(AdminUserFilterDto filter, CancellationToken cancellationToken)
        => (await userRepository.GetUsersAsync(filter.Role, filter.IsLocked, cancellationToken))
            .Select(user => new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsLocked = user.IsLocked
            })
            .ToArray();

    public async Task SetUserLockStateAsync(int userId, bool isLocked, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        await userRepository.SetLockedAsync(userId, isLocked, cancellationToken);
    }
}
