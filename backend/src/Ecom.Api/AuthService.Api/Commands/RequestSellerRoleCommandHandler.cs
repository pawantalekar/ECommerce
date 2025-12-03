using Ecom.Application.AuthService.Application.DTO;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthService.Api.Commands
{
    public class RequestSellerRoleCommandHandler : IRequestHandler<RequestSellerRoleCommand, CreateSellerRequestResponseDto>
    {
        private readonly AuthDbContext _db;
        private readonly IHttpContextAccessor _http;

        public RequestSellerRoleCommandHandler(AuthDbContext db, IHttpContextAccessor http)
        {
            _db = db;
            _http = http;
        }

        public async Task<CreateSellerRequestResponseDto> Handle(RequestSellerRoleCommand _, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _db.Users.FindAsync(userId);

            if (user == null) throw new UnauthorizedAccessException();
            if (user.Role is "Seller" or "Admin")
                return new("You are already a seller or admin.");

            var hasPending = await _db.SellerRequests
                .AnyAsync(r => r.UserId == userId && r.Status == "Pending", ct);

            if (hasPending)
                return new("You already have a pending seller request.");

            var request = new SellerRequest
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _db.SellerRequests.Add(request);
            await _db.SaveChangesAsync();

            return new("Seller request sent successfully. Waiting for admin approval.");
        }
    }
}
