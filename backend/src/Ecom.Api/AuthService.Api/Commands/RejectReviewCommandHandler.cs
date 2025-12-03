using Ecom.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Commands
{
    public class RejectReviewCommandHandler : IRequestHandler<RejectReviewCommand, Unit>
    {
        private readonly AuthDbContext _db;
        public RejectReviewCommandHandler(AuthDbContext db) => _db = db;

        public async Task<Unit> Handle(RejectReviewCommand cmd, CancellationToken ct)
        {
            var review = await _db.Reviews
                .FirstOrDefaultAsync(r => r.Id == cmd.ReviewId, ct);

            if (review == null)
                throw new KeyNotFoundException("Review not found");

            if (review.Status != "Pending")
                throw new InvalidOperationException("Only pending reviews can be rejected");

            review.Status = "Rejected";
            review.ApprovedById = cmd.AdminId;
            review.ApprovedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
