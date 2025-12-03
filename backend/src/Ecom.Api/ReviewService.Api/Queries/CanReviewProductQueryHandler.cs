using Ecom.Application.ReviewService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ReviewService.Api.Queries
{
    public class CanReviewProductQueryHandler : IRequestHandler<CanReviewProductQuery, bool>
    {
        private readonly IReviewRepository _repo;
        private readonly IHttpContextAccessor _http;

        public CanReviewProductQueryHandler(IReviewRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public Task<bool> Handle(CanReviewProductQuery query, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            return _repo.HasPurchasedAndNotReviewedAsync(userId, query.ProductId);
        }
    }
}
