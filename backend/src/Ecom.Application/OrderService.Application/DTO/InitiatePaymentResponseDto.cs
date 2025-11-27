using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.OrderService.Application.DTO
{
    public record InitiatePaymentResponseDto(string RazorpayOrderId, decimal Amount, string KeyId);
}
