using Ecom.Application.AuthService.Application.DTO;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AuthService.Api.Queries
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, UserProfileDto>
    {
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetMyProfileQueryHandler(AuthDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private static Guid? GetUserIdFromClaims(ClaimsPrincipal user)
        {
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        public async Task<UserProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var userId = GetUserIdFromClaims(_httpContextAccessor.HttpContext?.User);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not authenticated or UserId missing in claims");
            }

            var profile = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId.Value)
                .Select(u => new UserProfileDto
                {
                    FirstName = u.UserProfile.FirstName,
                    LastName = u.UserProfile.LastName,
                    Gender = u.UserProfile.Gender,
                    MobileNumber = u.UserProfile.MobileNumber,
                    Email = u.Email,
                    Role = u.Role
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (profile == null)
            {
                throw new KeyNotFoundException("User profile not found.");
            }

            return profile;
        }
    }
}
