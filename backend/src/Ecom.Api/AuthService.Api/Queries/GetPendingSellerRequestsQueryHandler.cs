using Ecom.Application.AuthService.Application.DTO;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Api.Queries
{
    public class GetPendingSellerRequestsQueryHandler
         : IRequestHandler<GetPendingSellerRequestsQuery, List<SellerRequestDto>>
    {
        private readonly AuthDbContext _db;
        public GetPendingSellerRequestsQueryHandler(AuthDbContext db) => _db = db;

        public async Task<List<SellerRequestDto>> Handle(GetPendingSellerRequestsQuery _, CancellationToken ct)
        {
            return await _db.SellerRequests
                .AsNoTracking()
                .Where(r => r.Status == "Pending")
                .Include(r => r.User)
                .OrderByDescending(r => r.RequestedAt)
                .Select(r => new SellerRequestDto(
                    r.Id,
                    r.UserId,
                    r.User.Name,
                    r.User.Email,
                    r.RequestedAt,
                    r.Status
                ))
                .ToListAsync(ct);
        }
    }
}
