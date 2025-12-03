using Ecom.Application.ReviewService.Application.DTO;
using MediatR;

namespace ReviewService.Api.Queries
{
    public record GetReviewsByProductQuery(Guid ProductId) : IRequest<List<ReviewDto>>;
}
