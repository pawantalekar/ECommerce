using Ecom.Application.ReviewService.Application.DTO;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Queries
{
    public class GetPendingReviewsQueryHandler
        : IRequestHandler<GetPendingReviewsQuery, List<PendingReviewDto>>
    {
        private readonly AuthDbContext _db;
        public GetPendingReviewsQueryHandler(AuthDbContext db) => _db = db;

        public async Task<List<PendingReviewDto>> Handle(GetPendingReviewsQuery request, CancellationToken ct)
        {
            return await _db.Reviews
                .AsNoTracking()
                .Where(r => r.Status == "Pending")
                .Include(r => r.User)
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new PendingReviewDto(
                    r.Id,
                    r.ProductId,
                    r.Product.Name,
                    r.UserId,
                    r.User.Name,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                ))
                .ToListAsync(ct);
        }
    }
}
