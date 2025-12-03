using Ecom.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Commands
{
    public class ApproveSellerRequestCommandHandler
        : IRequestHandler<ApproveSellerRequestCommand, Unit>
    {
        private readonly AuthDbContext _db;
        public ApproveSellerRequestCommandHandler(AuthDbContext db) => _db = db;

        public async Task<Unit> Handle(ApproveSellerRequestCommand cmd, CancellationToken ct)
        {
            var request = await _db.SellerRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == cmd.RequestId, ct);

            if (request == null)
                throw new KeyNotFoundException("Seller request not found");

            request.User.Role = "Seller";

            request.Status = "Approved";
            request.ApprovedById = cmd.AdminId;
            request.ApprovedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
