using CarMarketplace.Api.Dtos;

namespace CarMarketplace.Api.Services;

public interface IOrderService
{
    Task<OrderResponseDto> CreateAsync(int buyerId, CreateOrderRequestDto request, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<OrderResponseDto>> GetMineAsync(int buyerId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<OrderResponseDto>> GetSellerOrdersAsync(int sellerId, CancellationToken cancellationToken);

    Task<OrderResponseDto> UpdateStatusAsync(int orderId, int currentUserId, string currentUserRole, UpdateOrderStatusRequestDto request, CancellationToken cancellationToken);
}
