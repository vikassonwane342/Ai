using System.ComponentModel.DataAnnotations;

namespace CarMarketplace.Api.Dtos;

public class CreateOrderRequestDto
{
    [Range(1, int.MaxValue)]
    public int CarListingId { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class UpdateOrderStatusRequestDto
{
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class OrderResponseDto
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
