using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.OrderService.Application.Interfaces
{
    public interface IPaymentGateway
    {
        Task<string> CreatePaymentIntentAsync(decimal amount, string receipt);
        bool VerifyWebhookSignature(string payload, string signature, string secret);
    }
}
