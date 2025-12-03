using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Commands
{
    public class CreateSellerRequestCommandHandler
         : IRequestHandler<CreateSellerRequestCommand, Unit>
    {
        private readonly AuthDbContext _db;

        public CreateSellerRequestCommandHandler(AuthDbContext db) => _db = db;

        public async Task<Unit> Handle(CreateSellerRequestCommand request, CancellationToken ct)
        {
            
            var exists = await _db.SellerRequests
                .AnyAsync(r => r.UserId == request.UserId && r.Status == "Pending", ct);

            if (exists)
                throw new InvalidOperationException("You already have a pending seller request.");

            var sellerRequest = new SellerRequest
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _db.SellerRequests.Add(sellerRequest);
            await _db.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
