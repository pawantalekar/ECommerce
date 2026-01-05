using Ecom.Domain.Entities;
using System.Threading;

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
        Task<Address?> AddAddressAsync(Guid userId, Address address, CancellationToken ct);
        Task<List<Address?>> GetAddressesAsync(Guid userId,CancellationToken cancellationToken);
        Task<bool> UpdateAddressAsync(Guid addressId, string fullName, string phone, string addressLine1, string? addressLine2, string city, string state, string pincode, string country, string addressType, bool isDefault, CancellationToken ct);
        Task<bool> DeleteAddressAsync(Guid addressId, CancellationToken ct);
    }
}
