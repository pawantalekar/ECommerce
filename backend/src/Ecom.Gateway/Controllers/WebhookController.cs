using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Api.Commands;

namespace Ecom.Gateway.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class WebhookController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WebhookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Handle()
        {
            using var reader = new StreamReader(Request.Body);
            var payload = await reader.ReadToEndAsync();
            var signature = Request.Headers["X-Razorpay-Signature"].FirstOrDefault() ?? "";

            var command = new ProcessPaymentWebhookCommand(payload, signature);
            await _mediator.Send(command);
            return Ok();
        }
    }
}