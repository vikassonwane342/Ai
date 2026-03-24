namespace CarMarketplace.Api.Entities;

public class CarImage
{
    public int Id { get; set; }

    public int CarListingId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }
}
