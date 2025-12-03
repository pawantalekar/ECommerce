using Ecom.Application.OrderService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System.Text;

namespace Ecom.Infrastructure.Repository
{
    public class RazorpayGateway : IPaymentGateway
    {
        private readonly RazorpayClient _client;
        private readonly string _secret;

        public RazorpayGateway(IConfiguration config)
        {
            var key = config["Razorpay:KeyId"];
            _secret = config["Razorpay:KeySecret"];
            _client = new RazorpayClient(key, _secret);
        }

        public async Task<string> CreatePaymentIntentAsync(decimal amount, string receipt)
        {
            var options = new Dictionary<string, object>
            {
                ["amount"] = (int)(amount * 100),
                ["currency"] = "INR",
                ["receipt"] = receipt
            };

            var order =  _client.Order.Create(options);
            return order["id"].ToString();
        }

        public bool VerifyWebhookSignature(string payload, string signature, string secret)
        {
          
            using (var hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
                var generatedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return generatedSignature == signature;
            }
        }
    }
}
