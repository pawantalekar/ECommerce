using Ecom.Application.ReviewService.Application.DTO;
using MediatR;

namespace AuthService.Api.Queries
{
    public record GetPendingReviewsQuery : IRequest<List<PendingReviewDto>>;
}
