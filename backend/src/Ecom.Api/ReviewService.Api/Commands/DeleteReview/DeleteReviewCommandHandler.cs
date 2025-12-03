using Ecom.Application.ReviewService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ReviewService.Api.Commands.DeleteReview
{
    public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand>
    {
        private readonly IReviewRepository _repo;
        private readonly IHttpContextAccessor _http;

        public DeleteReviewCommandHandler(IReviewRepository repo, IHttpContextAccessor http)
        {
            _repo = repo; _http = http;
        }

        public async Task Handle(DeleteReviewCommand cmd, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var review = await _repo.GetByIdAsync(cmd.Id);

            if (review == null || review.UserId != userId)
                throw new InvalidOperationException("Not authorized");

            await _repo.DeleteAsync(review);
            await _repo.SaveChangesAsync();
        }
    }
}
