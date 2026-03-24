using System.ComponentModel.DataAnnotations;
using CarMarketplace.Api.Common;

namespace CarMarketplace.Api.Dtos;

public class CarSearchRequestDto
{
    public string? Keyword { get; set; }

    public string? Brand { get; set; }

    public string? Model { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MinPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? MaxPrice { get; set; }

    [Range(1900, 2100)]
    public int? Year { get; set; }

    public string? FuelType { get; set; }

    public string? Transmission { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}

public class CreateCarRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Brand { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int Mileage { get; set; }

    [Required]
    [MaxLength(50)]
    public string FuelType { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Transmission { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    public List<string> ImageUrls { get; set; } = [];
}

public class UpdateCarRequestDto : CreateCarRequestDto
{
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = AppConstants.ListingStatuses.Active;
}

public class CarListingResponseDto
{
    public int Id { get; set; }

    public int SellerId { get; set; }

    public string SellerName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public decimal Price { get; set; }

    public int Mileage { get; set; }

    public string FuelType { get; set; } = string.Empty;

    public string Transmission { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Status { get; set; } = string.Empty;

    public bool IsLocked { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public IReadOnlyCollection<string> ImageUrls { get; set; } = [];
}
