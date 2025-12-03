using Ecom.Domain.Entities;

namespace Ecom.Application.ReviewService.Application.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetByProductIdAsync(Guid productId, bool onlyApproved = false);
        Task<Review?> GetUserReviewForProductAsync(Guid userId, Guid productId);
        Task<bool> HasPurchasedAndNotReviewedAsync(Guid userId, Guid productId);
        Task<Review?> GetByIdAsync(Guid id);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
        Task<int> SaveChangesAsync();
    }
}
