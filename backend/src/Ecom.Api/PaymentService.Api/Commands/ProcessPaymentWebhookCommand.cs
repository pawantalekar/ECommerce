using MediatR;

namespace PaymentService.Api.Commands
{
    public record ProcessPaymentWebhookCommand(string Payload, string Signature) : IRequest;
}