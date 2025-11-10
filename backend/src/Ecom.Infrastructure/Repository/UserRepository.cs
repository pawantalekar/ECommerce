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
    }
}
