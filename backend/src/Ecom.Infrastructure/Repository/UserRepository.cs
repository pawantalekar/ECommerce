using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Infrastructure.Repository
{  
    public class UserRepository : IUserRepository
    {
        private readonly AuthDbContext _db;
        public UserRepository(AuthDbContext db) { _db = db; }
        public async Task<User> GetBySsoAsync(string provider, string providerId) => await _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Ssoprovider == provider && u.SsoproviderId == providerId);
        public async Task<User> GetByEmailAsync(string email) => await _db.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email);
        public async Task<User> AddAsync(User user) { _db.Users.Add(user); await _db.SaveChangesAsync(); return user; }
        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
        public async Task<UserProfile?> UpdateProfileAsync(
            Guid userId,
            string firstName,
            string lastName,
            string? gender,
            string? mobileNumber,
            CancellationToken ct)
        {
            var user = await _db.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null || user.UserProfile == null)
                return null;

            var profile = user.UserProfile;

            profile.FirstName = firstName;
            profile.LastName = lastName;
            profile.Gender = gender;
            profile.MobileNumber = mobileNumber ?? string.Empty;
            profile.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);

            return profile;
        }
    }
}
