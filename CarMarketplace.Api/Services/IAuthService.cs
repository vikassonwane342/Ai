using CarMarketplace.Api.Dtos;

namespace CarMarketplace.Api.Services;

public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken);

    Task<UserResponseDto> GetCurrentUserAsync(int userId, CancellationToken cancellationToken);
}
