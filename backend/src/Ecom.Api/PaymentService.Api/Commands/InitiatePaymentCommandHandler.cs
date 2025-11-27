using Ecom.Application.OrderService.Application.DTO;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaymentService.Api.Commands;
using Razorpay.Api;
using System.Security.Claims;
using EcomOrder = Ecom.Domain.Entities.Order;

namespace Ecom.Application.Commands.InitiatePayment
{
    public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, InitiatePaymentResponseDto>
    {
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly IConfiguration _config;

        public InitiatePaymentCommandHandler(AuthDbContext context, IHttpContextAccessor http, IConfiguration config)
        {
            _context = context;
            _http = http;
            _config = config;
        }

        public async Task<InitiatePaymentResponseDto> Handle(InitiatePaymentCommand request, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var razorpayClient = new RazorpayClient(_config["Authentication:Razorpay:KeyId"], _config["Authentication:Razorpay:KeySecret"]);

            decimal totalAmount = 0m;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId, ct)
                    ?? throw new Exception($"Product with ID {item.ProductId} not found");

                var thumbnailUrl = await _context.ProductImages
                    .AsNoTracking()
                    .Where(img => img.ProductId == item.ProductId && img.IsThumbnail== true)
                    .Select(img => img.Url)
                    .FirstOrDefaultAsync(ct);

                var unitPrice = product.Price;
                totalAmount += unitPrice * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ThumbnailUrl = thumbnailUrl,
                    UnitPrice = unitPrice,
                    Quantity = item.Quantity,
                    SubTotal = unitPrice * item.Quantity
                });
            }

            var orderOptions = new Dictionary<string, object>
            {
                ["amount"] = (int)(totalAmount * 100),
                ["currency"] = "INR",
                ["receipt"] = Guid.NewGuid().ToString()
            };

            var razorpayOrder = razorpayClient.Order.Create(orderOptions);
            var razorpayOrderId = razorpayOrder["id"].ToString();

            var order = new EcomOrder
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                OrderNumber = $"ORD-{DateTime.Now:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N").Substring(0, 4)}",
                TotalAmount = totalAmount,
                PaymentStatus = "Pending",
                OrderStatus = "Pending",
                ShippingFullName = request.ShippingAddress.FullName,
                ShippingPhone = request.ShippingAddress.Phone,
                ShippingAddressLine1 = request.ShippingAddress.AddressLine1,
                ShippingAddressLine2 = request.ShippingAddress.AddressLine2,
                ShippingCity = request.ShippingAddress.City,
                ShippingState = request.ShippingAddress.State,
                ShippingPincode = request.ShippingAddress.Pincode,
                RazorpayOrderId = razorpayOrderId,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(ct);

            return new InitiatePaymentResponseDto(
                RazorpayOrderId: razorpayOrderId,
                Amount: totalAmount,
                KeyId: _config["Authentication:Razorpay:KeyId"]!
            );
        }
    }
}