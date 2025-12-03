using Ecom.Application.ReviewService.Application.DTO;
using MediatR;

namespace ReviewService.Api.Queries
{
    public record GetMyReviewForProductQuery(Guid ProductId) : IRequest<ReviewDto?>;
}
