using MediatR;

namespace ReviewService.Api.Queries
{
    public record CanReviewProductQuery(Guid ProductId) : IRequest<bool>;
}
