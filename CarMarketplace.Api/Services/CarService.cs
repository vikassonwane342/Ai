using CarMarketplace.Api.Common;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Entities;
using CarMarketplace.Api.Repositories;

namespace CarMarketplace.Api.Services;

public class CarService(ICarRepository carRepository) : ICarService
{
    public async Task<PagedResponse<CarListingResponseDto>> SearchPublicAsync(CarSearchRequestDto request, CancellationToken cancellationToken)
    {
        var result = await carRepository.SearchPublicAsync(request, cancellationToken);
        return new PagedResponse<CarListingResponseDto>
        {
            Items = result.Items.Select(MapCar).ToArray(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<CarListingResponseDto> GetPublicByIdAsync(int id, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetPublicByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Car listing not found.");

        return MapCar(car);
    }

    public async Task<CarListingResponseDto> CreateAsync(int sellerId, CreateCarRequestDto request, CancellationToken cancellationToken)
    {
        var car = new CarListing
        {
            SellerId = sellerId,
            Title = request.Title.Trim(),
            Brand = request.Brand.Trim(),
            Model = request.Model.Trim(),
            Year = request.Year,
            Price = request.Price,
            Mileage = request.Mileage,
            FuelType = request.FuelType.Trim(),
            Transmission = request.Transmission.Trim(),
            Description = request.Description?.Trim(),
            Status = AppConstants.ListingStatuses.Active,
            Images = request.ImageUrls
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Select((url, index) => new CarImage
                {
                    ImageUrl = url.Trim(),
                    SortOrder = index + 1
                })
                .ToList()
        };

        car.Id = await carRepository.CreateAsync(car, cancellationToken);

        var created = await carRepository.GetByIdAsync(car.Id, cancellationToken)
            ?? throw new NotFoundException("Created car listing could not be loaded.");

        return MapCar(created);
    }

    public async Task<CarListingResponseDto> UpdateAsync(int carId, int currentUserId, string currentUserRole, UpdateCarRequestDto request, CancellationToken cancellationToken)
    {
        if (!AppConstants.ListingStatuses.Allowed.Contains(request.Status))
        {
            throw new BadRequestException("Invalid listing status.");
        }

        var existing = await carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new NotFoundException("Car listing not found.");

        EnsureOwnerOrAdmin(existing.SellerId, currentUserId, currentUserRole);

        existing.Title = request.Title.Trim();
        existing.Brand = request.Brand.Trim();
        existing.Model = request.Model.Trim();
        existing.Year = request.Year;
        existing.Price = request.Price;
        existing.Mileage = request.Mileage;
        existing.FuelType = request.FuelType.Trim();
        existing.Transmission = request.Transmission.Trim();
        existing.Description = request.Description?.Trim();
        existing.Status = request.Status.Trim();
        existing.Images = request.ImageUrls
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Select((url, index) => new CarImage
            {
                CarListingId = existing.Id,
                ImageUrl = url.Trim(),
                SortOrder = index + 1
            })
            .ToList();

        await carRepository.UpdateAsync(existing, cancellationToken);

        var updated = await carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new NotFoundException("Updated car listing could not be loaded.");

        return MapCar(updated);
    }

    public async Task DeleteAsync(int carId, int currentUserId, string currentUserRole, CancellationToken cancellationToken)
    {
        var existing = await carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new NotFoundException("Car listing not found.");

        EnsureOwnerOrAdmin(existing.SellerId, currentUserId, currentUserRole);
        await carRepository.DeleteAsync(carId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<CarListingResponseDto>> GetMineAsync(int sellerId, CancellationToken cancellationToken)
        => (await carRepository.GetBySellerAsync(sellerId, cancellationToken)).Select(MapCar).ToArray();

    public async Task<IReadOnlyCollection<CarListingResponseDto>> GetAllForAdminAsync(CancellationToken cancellationToken)
        => (await carRepository.GetAllAsync(cancellationToken)).Select(MapCar).ToArray();

    public async Task SetLockStateAsync(int carId, bool isLocked, CancellationToken cancellationToken)
    {
        var existing = await carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new NotFoundException("Car listing not found.");

        await carRepository.SetLockedAsync(carId, isLocked, cancellationToken);
    }

    private static void EnsureOwnerOrAdmin(int sellerId, int currentUserId, string currentUserRole)
    {
        if (currentUserRole == AppConstants.Roles.Admin)
        {
            return;
        }

        if (sellerId != currentUserId)
        {
            throw new ForbiddenException("You do not have access to this listing.");
        }
    }

    private static CarListingResponseDto MapCar(CarListing car) => new()
    {
        Id = car.Id,
        SellerId = car.SellerId,
        SellerName = car.SellerName,
        Title = car.Title,
        Brand = car.Brand,
        Model = car.Model,
        Year = car.Year,
        Price = car.Price,
        Mileage = car.Mileage,
        FuelType = car.FuelType,
        Transmission = car.Transmission,
        Description = car.Description,
        Status = car.Status,
        IsLocked = car.IsLocked,
        CreatedAt = car.CreatedAt,
        UpdatedAt = car.UpdatedAt,
        ImageUrls = car.Images
            .OrderBy(image => image.SortOrder)
            .Select(image => image.ImageUrl)
            .ToArray()
    };
}
