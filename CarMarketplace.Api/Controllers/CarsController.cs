using CarMarketplace.Api.Common;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarMarketplace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController(ICarService carService, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<PagedResponse<CarListingResponseDto>>>> Search([FromQuery] CarSearchRequestDto request, CancellationToken cancellationToken)
    {
        var cars = await carService.SearchPublicAsync(request, cancellationToken);
        return Ok(ApiResponse<PagedResponse<CarListingResponseDto>>.SuccessResponse(cars));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<CarListingResponseDto>>> GetById(int id, CancellationToken cancellationToken)
    {
        var car = await carService.GetPublicByIdAsync(id, cancellationToken);
        return Ok(ApiResponse<CarListingResponseDto>.SuccessResponse(car));
    }

    [HttpPost]
    [Authorize(Roles = AppConstants.Roles.Seller)]
    public async Task<ActionResult<ApiResponse<CarListingResponseDto>>> Create(CreateCarRequestDto request, CancellationToken cancellationToken)
    {
        var car = await carService.CreateAsync(currentUserService.UserId, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = car.Id }, ApiResponse<CarListingResponseDto>.SuccessResponse(car, "Car listing created successfully."));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{AppConstants.Roles.Seller},{AppConstants.Roles.Admin}")]
    public async Task<ActionResult<ApiResponse<CarListingResponseDto>>> Update(int id, UpdateCarRequestDto request, CancellationToken cancellationToken)
    {
        var car = await carService.UpdateAsync(id, currentUserService.UserId, currentUserService.Role, request, cancellationToken);
        return Ok(ApiResponse<CarListingResponseDto>.SuccessResponse(car, "Car listing updated successfully."));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = $"{AppConstants.Roles.Seller},{AppConstants.Roles.Admin}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken cancellationToken)
    {
        await carService.DeleteAsync(id, currentUserService.UserId, currentUserService.Role, cancellationToken);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Car listing deleted successfully."));
    }

    [HttpGet("mine")]
    [Authorize(Roles = AppConstants.Roles.Seller)]
    public async Task<ActionResult<ApiResponse<IReadOnlyCollection<CarListingResponseDto>>>> Mine(CancellationToken cancellationToken)
    {
        var cars = await carService.GetMineAsync(currentUserService.UserId, cancellationToken);
        return Ok(ApiResponse<IReadOnlyCollection<CarListingResponseDto>>.SuccessResponse(cars));
    }
}
