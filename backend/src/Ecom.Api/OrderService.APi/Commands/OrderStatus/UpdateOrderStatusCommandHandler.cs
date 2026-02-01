using Ecom.Application.OrderService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OrderService.APi.Commands.OrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _http;

        private static readonly string[] ValidStatuses =
        [
            "Pending", "Confirmed", "Processing", "Shipped",
            "OutForDelivery", "Delivered", "Cancelled", "Failed"
        ];

        public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IHttpContextAccessor http)
        {
            _orderRepository = orderRepository;
            _http = http;
        }

        public async Task<UpdateOrderStatusResult> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
        {
            if (!ValidStatuses.Contains(request.NewStatus))
                throw new ArgumentException($"Invalid status. Allowed: {string.Join(", ", ValidStatuses)}");

            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? adminUserId = userIdClaim != null && Guid.TryParse(userIdClaim, out var uid) ? uid : null;

            var order = await _orderRepository.GetOrderByNumberAsync(request.OrderNumber, ct)
                ?? throw new KeyNotFoundException($"Order {request.OrderNumber} not found");

            if (order.OrderStatus == request.NewStatus)
                throw new InvalidOperationException($"Order is already in {request.NewStatus} status");

            await _orderRepository.UpdateOrderStatusAsync(order, request.NewStatus, adminUserId, request.Notes, ct);

            return new UpdateOrderStatusResult(
                order.OrderNumber,
                order.OrderStatus,
                DateTime.UtcNow
            );
        }
    }
}
