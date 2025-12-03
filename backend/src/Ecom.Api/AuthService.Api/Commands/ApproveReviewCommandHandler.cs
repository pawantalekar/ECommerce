using Ecom.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Commands
{
    public class ApproveReviewCommandHandler : IRequestHandler<ApproveReviewCommand, Unit>
    {
        private readonly AuthDbContext _db;
        public ApproveReviewCommandHandler(AuthDbContext db) => _db = db;

        public async Task<Unit> Handle(ApproveReviewCommand cmd, CancellationToken ct)
        {
            var review = await _db.Reviews
                .FirstOrDefaultAsync(r => r.Id == cmd.ReviewId, ct);

            if (review == null)
                throw new KeyNotFoundException("Review not found");

            if (review.Status != "Pending")
                throw new InvalidOperationException("Only pending reviews can be approved");

            review.Status = "Approved";
            review.ApprovedById = cmd.AdminId;
            review.ApprovedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
