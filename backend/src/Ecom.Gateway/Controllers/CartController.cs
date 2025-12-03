using CartService.Api.Commands.AddToCart;
using CartService.Api.Commands.RemoveFromCart;
using CartService.Api.Commands.UpdateCart;
using CartService.Api.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Gateway.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<GetMyCartQueryResult>> GetMyCart()
        {
            var result = await _mediator.Send(new GetMyCartQuery());
            return Ok(result);
        }
        [Authorize]
        [HttpPost("items")]
        public async Task<ActionResult<CartService.Api.Commands.AddToCart.AddToCartCommandResult>> AddToCart([FromBody] AddToCartCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("items/{productId}")]
        public async Task<ActionResult<UpdateCartItemCommandResult>> UpdateCartItem(
            Guid productId,
            [FromBody] UpdateCartItemCommand command)
        {
            command.ProductId = productId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        
        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<RemoveFromCartCommandResult>> RemoveFromCart(Guid productId)
        {
            var result = await _mediator.Send(new RemoveFromCartCommand { ProductId = productId });
            return Ok(result);
        }
    }
}