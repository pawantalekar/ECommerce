using Ecom.Application.ReviewService.Application.DTO;
using MediatR;

namespace ReviewService.Api.Commands.UpdateReview
{
    public record UpdateReviewCommand(Guid Id, UpdateReviewRequestDto Request) : IRequest<ReviewDto>;
}
