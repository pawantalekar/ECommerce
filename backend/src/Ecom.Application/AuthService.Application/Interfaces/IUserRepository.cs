using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.AuthService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetBySsoAsync(string provider, string providerId);
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task SaveChangesAsync();
    }
}
