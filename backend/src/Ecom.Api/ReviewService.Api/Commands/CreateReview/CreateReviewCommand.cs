using Ecom.Application.ReviewService.Application.DTO;
using MediatR;

namespace ReviewService.Api.Commands.CreateReview
{
    public record CreateReviewCommand(CreateReviewRequestDto Request) : IRequest<ReviewDto>;
}
