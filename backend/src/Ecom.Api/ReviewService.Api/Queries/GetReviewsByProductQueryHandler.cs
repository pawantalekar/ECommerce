using Ecom.Application.ReviewService.Application.DTO;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Infrastructure;
using MediatR;

namespace ReviewService.Api.Queries
{
    public class GetReviewsByProductQueryHandler : IRequestHandler<GetReviewsByProductQuery, List<ReviewDto>>
    {
        private readonly IReviewRepository _repo;
        private readonly AuthDbContext _context;

        public GetReviewsByProductQueryHandler(IReviewRepository repo, AuthDbContext context)
        {
            _repo = repo; _context = context;
        }

        public async Task<List<ReviewDto>> Handle(GetReviewsByProductQuery query, CancellationToken ct)
        {
            var reviews = await _repo.GetByProductIdAsync(query.ProductId, onlyApproved: true);
            return reviews.Select(r => new ReviewDto(
                r.Id, r.ProductId, r.UserId, r.Rating, r.Comment, r.CreatedAt, r.UpdatedAt,
                r.User?.Name ?? "Anonymous"
            )).ToList();
        }
    }
}
