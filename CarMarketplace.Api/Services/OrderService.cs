using CarMarketplace.Api.Common;
using CarMarketplace.Api.Dtos;
using CarMarketplace.Api.Entities;
using CarMarketplace.Api.Repositories;

namespace CarMarketplace.Api.Services;

public class OrderService(IOrderRepository orderRepository, ICarRepository carRepository) : IOrderService
{
    public async Task<OrderResponseDto> CreateAsync(int buyerId, CreateOrderRequestDto request, CancellationToken cancellationToken)
    {
        var car = await carRepository.GetByIdAsync(request.CarListingId, cancellationToken)
            ?? throw new NotFoundException("Car listing not found.");

        if (car.SellerId == buyerId)
        {
            throw new ConflictException("You cannot book your own listing.");
        }

        if (car.IsLocked || car.Status != AppConstants.ListingStatuses.Active)
        {
            throw new ConflictException("This listing cannot be booked.");
        }

        var order = new Order
        {
            CarListingId = request.CarListingId,
            BuyerId = buyerId,
            SellerId = car.SellerId,
            Status = AppConstants.OrderStatuses.Pending,
            Notes = request.Notes?.Trim()
        };

        order.Id = await orderRepository.CreateAsync(order, cancellationToken);

        var created = await orderRepository.GetByIdAsync(order.Id, cancellationToken)
            ?? throw new NotFoundException("Created order could not be loaded.");

        return MapOrder(created);
    }

    public async Task<IReadOnlyCollection<OrderResponseDto>> GetMineAsync(int buyerId, CancellationToken cancellationToken)
        => (await orderRepository.GetByBuyerIdAsync(buyerId, cancellationToken)).Select(MapOrder).ToArray();

    public async Task<IReadOnlyCollection<OrderResponseDto>> GetSellerOrdersAsync(int sellerId, CancellationToken cancellationToken)
        => (await orderRepository.GetBySellerIdAsync(sellerId, cancellationToken)).Select(MapOrder).ToArray();

    public async Task<OrderResponseDto> UpdateStatusAsync(int orderId, int currentUserId, string currentUserRole, UpdateOrderStatusRequestDto request, CancellationToken cancellationToken)
    {
        if (!AppConstants.OrderStatuses.AllowedUpdates.Contains(request.Status))
        {
            throw new BadRequestException("Invalid order status.");
        }

        var order = await orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException("Order not found.");

        if (currentUserRole != AppConstants.Roles.Admin && order.SellerId != currentUserId)
        {
            throw new ForbiddenException("You do not have access to this order.");
        }

        if (order.Status != AppConstants.OrderStatuses.Pending)
        {
            throw new ConflictException("Only pending orders can be updated.");
        }

        await orderRepository.UpdateStatusAsync(orderId, request.Status.Trim(), request.Notes?.Trim(), cancellationToken);

        var updated = await orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException("Updated order could not be loaded.");

        return MapOrder(updated);
    }

    private static OrderResponseDto MapOrder(Order order) => new()
    {
        Id = order.Id,
        CarListingId = order.CarListingId,
        BuyerId = order.BuyerId,
        SellerId = order.SellerId,
        Status = order.Status,
        Notes = order.Notes,
        BookedAt = order.BookedAt,
        UpdatedAt = order.UpdatedAt,
        BuyerName = order.BuyerName,
        SellerName = order.SellerName,
        CarTitle = order.CarTitle
    };
}
