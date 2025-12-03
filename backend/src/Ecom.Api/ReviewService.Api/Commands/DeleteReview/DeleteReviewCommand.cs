using MediatR;

namespace ReviewService.Api.Commands.DeleteReview
{
    public record DeleteReviewCommand(Guid Id) : IRequest;
}
