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

            if (user == null)
                return null;

            var profile = user.UserProfile;

            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    FirstName = firstName,
                    LastName = lastName,
                    Gender = gender,
                    MobileNumber = mobileNumber ?? string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                user.UserProfile = profile;
                _db.UserProfiles.Add(profile);
            }
            else
            {
                profile.FirstName = firstName;
                profile.LastName = lastName;
                profile.Gender = gender;
                profile.MobileNumber = mobileNumber ?? string.Empty;
                profile.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync(ct);

            return profile;
        }
        public async Task<Address?> AddAddressAsync(Guid userId, Address address, CancellationToken ct)
        {
            var user = await _db.Users
                .Include(u => u.UserProfile)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user == null || user.UserProfile == null)
                return null;

            // Safe: ExecuteUpdateAsync returns 0 when no rows affected — no exception
            if (address.IsDefault)
            {
                await _db.Addresses
                    .Where(a => a.UserProfileId == user.UserProfile.UserId)
                    .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false), ct);
            }

            address.UserProfileId = user.UserProfile.UserId;
            address.CreatedAt = DateTime.UtcNow;
            address.UpdatedAt = DateTime.UtcNow;

            // Add directly to DbSet — NO COLLECTION USAGE
            _db.Addresses.Add(address);

            await _db.SaveChangesAsync(ct);

            return address;
        }
        public async Task<List<Address?>> GetAddressesAsync(Guid userId, CancellationToken cancellationToken)
        {
           var result =  await _db.Addresses.Where(a => a.UserProfileId == userId).ToListAsync();

            return result;
        }
        public async Task<bool> UpdateAddressAsync(Guid addressId, string fullName, string phone, string addressLine1, string? addressLine2, string city, string state, string pincode, string country, string addressType, bool isDefault, CancellationToken ct)
        {
                    var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == addressId, ct);
                    if (address == null)
                        return false;

                    address.FullName = fullName;
                    address.Phone = phone;
                    address.AddressLine1 = addressLine1;
                    address.AddressLine2 = addressLine2;
                    address.City = city;
                    address.State = state;
                    address.Pincode = pincode;
                    address.Country = country;
                    address.AddressType = addressType;
                    address.IsDefault = isDefault;
                    address.UpdatedAt = DateTime.UtcNow;

                    if (isDefault)
                    {
                        await _db.Addresses
                            .Where(a => a.UserProfileId == address.UserProfileId && a.Id != address.Id)
                            .ExecuteUpdateAsync(s => s.SetProperty(a => a.IsDefault, false), ct);
                    }

                    await _db.SaveChangesAsync(ct);
                    return true;
        }
        public async Task<bool> DeleteAddressAsync(Guid addressId, CancellationToken ct)
        {
            var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == addressId, ct);
            if (address == null)
                return false;

            _db.Addresses.Remove(address);
            await _db.SaveChangesAsync(ct);
            return true;
        }

    }
}
