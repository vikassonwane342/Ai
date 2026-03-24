using CarMarketplace.Api.Entities;

namespace CarMarketplace.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<int> CreateAsync(User user, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<User>> GetUsersAsync(string? role, bool? isLocked, CancellationToken cancellationToken);

    Task SetLockedAsync(int id, bool isLocked, CancellationToken cancellationToken);
}
