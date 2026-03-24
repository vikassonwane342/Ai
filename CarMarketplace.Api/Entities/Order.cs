namespace CarMarketplace.Api.Entities;

public class Order
{
    public int Id { get; set; }

    public int CarListingId { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DateTime BookedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string BuyerName { get; set; } = string.Empty;

    public string SellerName { get; set; } = string.Empty;

    public string CarTitle { get; set; } = string.Empty;
}
