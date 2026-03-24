namespace CarMarketplace.Api.Entities;

public class CarListing
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

    public List<CarImage> Images { get; set; } = [];
}
