using Ecom.Application.ReviewService.Application.DTO;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ReviewService.Api.Commands.UpdateReview
{
    public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
    {
        private readonly IReviewRepository _repo;
        private readonly IHttpContextAccessor _http;
        private readonly AuthDbContext _context;

        public UpdateReviewCommandHandler(IReviewRepository repo, IHttpContextAccessor http, AuthDbContext context)
        {
            _repo = repo; _http = http; _context = context;
        }

        public async Task<ReviewDto> Handle(UpdateReviewCommand cmd, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var review = await _repo.GetByIdAsync(cmd.Id);

            if (review == null || review.UserId != userId)
                throw new InvalidOperationException("Not authorized");

            review.Rating = cmd.Request.Rating;
            review.Comment = cmd.Request.Comment;
            review.Status = "Pending";
            review.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(review);
            await _repo.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            return new ReviewDto(review.Id, review.ProductId, review.UserId, review.Rating, review.Comment, review.CreatedAt, review.UpdatedAt, user!.Name);
        }
    }
}
