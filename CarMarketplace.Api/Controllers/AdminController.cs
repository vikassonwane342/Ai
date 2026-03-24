using CarMarketplace.Api.Common;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarMarketplace.Api.Controllers;

[ApiController]
[Authorize(Roles = AppConstants.Roles.Admin)]
[Route("api/admin")]
public class AdminController(IAdminService adminService, ICarService carService) : ControllerBase
{
    [HttpGet("users")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<UserResponseDto>>>> GetUsers([FromQuery] AdminUserFilterDto request, CancellationToken cancellationToken)
    {
        var users = await adminService.GetUsersAsync(request, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<UserResponseDto>>.SuccessResponse(users));
    }

    [HttpPatch("users/{id:int}/lock")]
    public async Task<ActionResult<ApiResponse<object>>> LockUser(int id, CancellationToken cancellationToken)
    {
        await adminService.SetUserLockStateAsync(id, true, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "User locked successfully."));
    }

    [HttpPatch("users/{id:int}/unlock")]
    public async Task<ActionResult<ApiResponse<object>>> UnlockUser(int id, CancellationToken cancellationToken)
    {
        await adminService.SetUserLockStateAsync(id, false, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "User unlocked successfully."));
    }

    [HttpGet("listings")]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CarListingResponseDto>>>> GetListings(CancellationToken cancellationToken)
    {
        var listings = await carService.GetAllForAdminAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<CarListingResponseDto>>.SuccessResponse(listings));
    }

    [HttpPatch("listings/{id:int}/lock")]
    public async Task<ActionResult<ApiResponse<object>>> LockListing(int id, CancellationToken cancellationToken)
    {
        await carService.SetLockStateAsync(id, true, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Listing locked successfully."));
    }

    [HttpPatch("listings/{id:int}/unlock")]
    public async Task<ActionResult<ApiResponse<object>>> UnlockListing(int id, CancellationToken cancellationToken)
    {
        await carService.SetLockStateAsync(id, false, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Listing unlocked successfully."));
    }
}
