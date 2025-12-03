using Ecom.Application.ReviewService.Application.DTO;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ReviewService.Api.Queries
{
    public class GetMyReviewForProductQueryHandler : IRequestHandler<GetMyReviewForProductQuery, ReviewDto?>
    {
        private readonly IReviewRepository _repo;
        private readonly IHttpContextAccessor _http;
        private readonly AuthDbContext _context;

        public GetMyReviewForProductQueryHandler(IReviewRepository repo, IHttpContextAccessor http, AuthDbContext _context)
        {
            _repo = repo;
            _http = http;
            this._context = _context;
        }

        public async Task<ReviewDto?> Handle(GetMyReviewForProductQuery query, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var review = await _repo.GetUserReviewForProductAsync(userId, query.ProductId);
            if (review == null) return null;

            return new ReviewDto(review.Id, review.ProductId, review.UserId, review.Rating, review.Comment, review.CreatedAt, review.UpdatedAt, _context.Users.Find(userId).Name);
        }
    }
}
