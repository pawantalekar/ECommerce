using Ecom.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PaymentService.Api.Commands
{
    public class ProcessPaymentWebhookCommandHandler : IRequestHandler<ProcessPaymentWebhookCommand>
    {
        private readonly AuthDbContext _context;
        private readonly string _webhookSecret;

        public ProcessPaymentWebhookCommandHandler(AuthDbContext context, IConfiguration config)
        {
            _context = context;
            _webhookSecret = config["Authentication:Razorpay:WebhookSecret"] ?? config["Authentication:Razorpay:KeySecret"]!;
        }

        public async Task Handle(ProcessPaymentWebhookCommand request, CancellationToken ct)
        {
            var computedSignature = ComputeHmacSha256(request.Payload, _webhookSecret);

            if (computedSignature != request.Signature.ToLower())
                return;

            var json = JsonSerializer.Deserialize<JsonElement>(request.Payload);
            var @event = json.GetProperty("event").GetString();

            if (@event != "payment.captured" && @event != "payment.failed")
                return;

            var razorpayOrderId = json.GetProperty("payload")
                .GetProperty("payment")
                .GetProperty("entity")
                .GetProperty("order_id")
                .GetString()!;

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.RazorpayOrderId == razorpayOrderId, ct);

            if (order == null)
                return;

            if (@event == "payment.captured")
            {
                order.PaymentStatus = "Paid";
                order.OrderStatus = "Confirmed";
                order.RazorpayPaymentId = json.GetProperty("payload")
                    .GetProperty("payment")
                    .GetProperty("entity")
                    .GetProperty("id")
                    .GetString();

                foreach (var item in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                        product.StockQuantity -= item.Quantity;
                }
            }
            else if (@event == "payment.failed")
            {
                order.PaymentStatus = "Failed";
                order.OrderStatus = "Failed";
            }

            await _context.SaveChangesAsync(ct);
        }

        private static string ComputeHmacSha256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}