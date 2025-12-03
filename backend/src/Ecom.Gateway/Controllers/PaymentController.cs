using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Api.Commands;

namespace Ecom.Gateway.Controllers
{
    [Route("api/payments")]
    [Authorize]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentController(IMediator mediator) => _mediator = mediator;

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] InitiatePaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}

