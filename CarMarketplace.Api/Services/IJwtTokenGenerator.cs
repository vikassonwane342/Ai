using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Entities;

namespace CarMarketplace.Api.Services;

public interface IJwtTokenGenerator
{
    AuthResponseDto GenerateToken(User user);
}
