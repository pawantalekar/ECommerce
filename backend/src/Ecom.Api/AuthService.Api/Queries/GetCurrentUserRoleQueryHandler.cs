using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthService.Api.Queries
{
    public class GetCurrentUserRoleQueryHandler : IRequestHandler<GetCurrentUserRoleQuery, string>
    {
        private readonly AuthDbContext _db;
        private readonly IHttpContextAccessor _http;

        public GetCurrentUserRoleQueryHandler(AuthDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public async Task<string> Handle(GetCurrentUserRoleQuery request, CancellationToken ct)
        {
            var userIdClaim = _http.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? _http.HttpContext?.User.FindFirst("sub");

            if (userIdClaim == null) return "User";

            var userId = Guid.Parse(userIdClaim.Value);
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            return user?.Role ?? "User";
        }
    }
}
