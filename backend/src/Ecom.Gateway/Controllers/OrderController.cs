using AuthService.Api.Queries.GetOrders;
using Ecom.Application.AuthService.Application.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderService.APi.Commands.OrderStatus;
using OrderService.APi.Queries;

namespace Ecom.Gateway.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var result = await _mediator.Send(new GetMyOrdersQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOrderDetailsQuery(id));
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<ActionResult<PagedOrdersDto>> GetAllOrdersForAdmin(
            [FromQuery] string[]? orderStatus = null,
            [FromQuery] string[]? paymentStatus = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetAllOrdersForAdminQuery(
                orderStatus, paymentStatus, fromDate, toDate, searchTerm, pageNumber, pageSize));
            return Ok(result);
        }
        [HttpPatch("admin/orders/{orderNumber}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(string orderNumber, [FromBody] UpdateOrderStatusRequest request)
        {
            var command = new UpdateOrderStatusCommand(orderNumber, request.Status, request.Notes);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
