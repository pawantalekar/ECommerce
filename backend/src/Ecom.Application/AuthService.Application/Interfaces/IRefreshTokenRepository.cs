using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.AuthService.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task RevokeAsync(RefreshToken token, string revokedByIp);
        Task SaveChangesAsync();
    }
}
