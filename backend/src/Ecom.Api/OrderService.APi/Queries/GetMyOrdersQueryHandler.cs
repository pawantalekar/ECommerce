using Ecom.Application.OrderService.Application.DTO;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OrderService.APi.Queries;
using System.Security.Claims;

namespace Ecom.Application.Queries
{
    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, List<OrderDto>>
    {
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _http;

        public GetMyOrdersQueryHandler(AuthDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task<List<OrderDto>> Handle(GetMyOrdersQuery request, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            return await _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDto(
                    o.Id,
                    o.OrderNumber,
                    o.TotalAmount,
                    o.PaymentStatus.ToString(),
                    o.OrderStatus.ToString(),
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
                    null
                ))
                .ToListAsync(ct);
        }
    }
}