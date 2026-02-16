using Ecom.Application.OrderService.Application.Interfaces;
using Ecom.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace OrderService.APi.Commands.OrderStatus
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, UpdateOrderStatusResult>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IHttpContextAccessor _http;

        public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository, IHttpContextAccessor http)
        {
            _orderRepository = orderRepository;
            _http = http;
        }

        public async Task<UpdateOrderStatusResult> Handle(UpdateOrderStatusCommand request, CancellationToken ct)
        {
            if (!Enum.TryParse<OrderStatusEnum>(request.NewStatus, out var newStatusEnum))
                throw new ArgumentException($"Invalid status. Allowed: {string.Join(", ", Enum.GetNames<OrderStatusEnum>())}");

            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid? adminUserId = userIdClaim != null && Guid.TryParse(userIdClaim, out var uid) ? uid : null;

            var order = await _orderRepository.GetOrderByNumberAsync(request.OrderNumber, ct)
                ?? throw new KeyNotFoundException($"Order {request.OrderNumber} not found");

            if (order.OrderStatus == newStatusEnum)
                throw new InvalidOperationException($"Order is already in {request.NewStatus} status");

            await _orderRepository.UpdateOrderStatusAsync(order, newStatusEnum, adminUserId, request.Notes, ct);

            return new UpdateOrderStatusResult(
                order.OrderNumber,
                order.OrderStatus.ToString(),
                DateTime.UtcNow
            );
        }
    }
}
