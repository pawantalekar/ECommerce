using Ecom.Application.OrderService.Application.DTO;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace OrderService.APi.Queries
{

    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, OrderDto>
    {
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _http;

        public GetOrderDetailsQueryHandler(AuthDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task<OrderDto> Handle(GetOrderDetailsQuery request, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.StatusHistory)
                .Where(o => o.Id == request.OrderId && o.UserId == userId)
                .Select(o => new OrderDto(
                    o.Id,
                    o.OrderNumber,
                    o.TotalAmount,
                    o.PaymentStatus,
                    o.OrderStatus,
                    o.CreatedAt,
                    o.OrderItems.Select(oi => new OrderItemDto(
                        oi.ProductId.ToString(),
                        oi.ProductName,
                        oi.ThumbnailUrl,
                        oi.UnitPrice,
                        oi.Quantity
                    )).ToList(),
                    new ShippingAddressDto(
                        o.ShippingFullName,
                        o.ShippingPhone,
                        o.ShippingAddressLine1,
                        o.ShippingAddressLine2,
                        o.ShippingCity,
                        o.ShippingState,
                        o.ShippingPincode,
                        o.ShippingAddressType,
                        o.ShippingCountry
                    ),
                    o.StatusHistory.OrderBy(sh => sh.ChangedAt)
                        .Select(sh => new OrderStatusHistoryDto(
                            sh.Status,
                            sh.ChangedAt,
                            sh.Notes
                        )).ToList()
                ))
                .FirstOrDefaultAsync(ct);

            if (order == null)
                throw new KeyNotFoundException("Order not found");

            return order;
        }
    }
}
