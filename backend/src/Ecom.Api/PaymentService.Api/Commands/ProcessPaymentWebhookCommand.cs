using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Api.Commands
{
    public record ProcessPaymentWebhookCommand(string Payload, string Signature) : IRequest;
}