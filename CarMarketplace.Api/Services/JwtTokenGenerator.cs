using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CarMarketplace.Api.Configuration;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CarMarketplace.Api.Services;

public class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public AuthResponseDto GenerateToken(User user)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}".Trim())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtOptions.Issuer,
            _jwtOptions.Audience,
            claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new AuthResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAt,
            User = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsLocked = user.IsLocked
            }
        };
    }
}
