using Ecom.Application.OrderService.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Api.Commands
{
    public record InitiatePaymentCommand(
        List<CheckoutItemDto> Items,
        ShippingAddressDto ShippingAddress
    ) : IRequest<InitiatePaymentResponseDto>;
}
