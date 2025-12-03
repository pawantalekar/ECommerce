using Ecom.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Commands
{
    public class RejectSellerRequestCommandHandler
         : IRequestHandler<RejectSellerRequestCommand, Unit>
    {
        private readonly AuthDbContext _db;
        public RejectSellerRequestCommandHandler(AuthDbContext db) => _db = db;

        public async Task<Unit> Handle(RejectSellerRequestCommand cmd, CancellationToken ct)
        {
            var request = await _db.SellerRequests
                .FirstOrDefaultAsync(r => r.Id == cmd.RequestId, ct);

            if (request == null) throw new KeyNotFoundException("Seller request not found");

            
            request.Status = "Rejected";
            request.ApprovedById = cmd.AdminId;
            request.ApprovedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
