using CarMarketplace.Api.Common;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Entities;
using CarMarketplace.Api.Repositories;

namespace CarMarketplace.Api.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : IAuthService
{
    public async Task<UserResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken)
    {
        if (!AppConstants.Roles.AllowedRegistrationRoles.Contains(request.Role))
        {
            throw new BadRequestException("Only Buyer and Seller roles can be registered.");
        }

        var existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone?.Trim(),
            PasswordHash = passwordHasher.HashPassword(request.Password),
            Role = request.Role.Trim(),
            IsLocked = false
        };

        user.Id = await userRepository.CreateAsync(user, cancellationToken);

        return MapUser(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new UnauthorizedAppException("Invalid email or password.");

        if (user.IsLocked)
        {
            throw new ForbiddenException("This user account is locked.");
        }

        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAppException("Invalid email or password.");
        }

        return jwtTokenGenerator.GenerateToken(user);
    }

    public async Task<UserResponseDto> GetCurrentUserAsync(int userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new NotFoundException("User not found.");

        return MapUser(user);
    }

    private static UserResponseDto MapUser(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Phone = user.Phone,
        Role = user.Role,
        IsLocked = user.IsLocked
    };
}
