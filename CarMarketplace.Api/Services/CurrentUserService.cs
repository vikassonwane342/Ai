using System.Security.Claims;

namespace CarMarketplace.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : 0;
        }
    }

    public string Role => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
}
