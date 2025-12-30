using Ecom.Domain.Entities;

namespace Ecom.Application.AuthService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetBySsoAsync(string provider, string providerId);
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task SaveChangesAsync();
        Task<UserProfile?> UpdateProfileAsync(
            Guid userId,
            string firstName,
            string lastName,
            string? gender,
            string? mobileNumber,
            CancellationToken cancellationToken);
    }
}
