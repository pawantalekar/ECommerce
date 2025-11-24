using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repository
{
    
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AuthDbContext _db;
        public RefreshTokenRepository(AuthDbContext db) { _db = db; }
        public async Task AddAsync(RefreshToken token) { _db.RefreshTokens.Add(token); await _db.SaveChangesAsync(); }
        public async Task<RefreshToken> GetByTokenAsync(string token) => await _db.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token);
        public async Task RevokeAsync(RefreshToken token, string revokedByIp) { token.RevokedAt = DateTime.UtcNow; token.CreatedByIp = revokedByIp; await _db.SaveChangesAsync(); }
        public async Task SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
