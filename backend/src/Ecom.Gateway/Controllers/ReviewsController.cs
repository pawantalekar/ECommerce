using Ecom.Application.ReviewService.Application.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewService.Api.Commands.CreateReview;
using ReviewService.Api.Commands.DeleteReview;
using ReviewService.Api.Commands.UpdateReview;
using ReviewService.Api.Queries;

namespace Ecom.Gateway.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByProduct(Guid productId)
        {
            var result = await _mediator.Send(new GetReviewsByProductQuery(productId));
            return Ok(result);
        }

        [HttpGet("can-review/{productId}")]
        public async Task<IActionResult> CanReview(Guid productId)
        {
            var result = await _mediator.Send(new CanReviewProductQuery(productId));
            return Ok(result);
        }

        [HttpGet("my/{productId}")]
        public async Task<IActionResult> GetMyReview(Guid productId)
        {
            var result = await _mediator.Send(new GetMyReviewForProductQuery(productId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequestDto request)
        {
            var result = await _mediator.Send(new CreateReviewCommand(request));
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewRequestDto request)
        {
            var result = await _mediator.Send(new UpdateReviewCommand(id, request));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteReviewCommand(id));
            return NoContent();
        }
    }
}
