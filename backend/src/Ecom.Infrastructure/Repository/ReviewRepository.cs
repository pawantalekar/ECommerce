using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Infrastructure.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AuthDbContext _context;

        public ReviewRepository(AuthDbContext context) => _context = context;

        public async Task<List<Review>> GetByProductIdAsync(Guid productId, bool onlyApproved = false)
        {
            var query = _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId);

            if (onlyApproved)
                query = query.Where(r => r.Status == "Approved");

            return await query
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public Task<Review?> GetUserReviewForProductAsync(Guid userId, Guid productId)
            => _context.Reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);

        public async Task<bool> HasPurchasedAndNotReviewedAsync(Guid userId, Guid productId)
        {
            var purchased = await _context.OrderItems
                .AnyAsync(oi => oi.Order.UserId == userId &&
                                oi.ProductId == productId &&
                                oi.Order.PaymentStatus == PaymentStatusEnum.Paid);

            if (!purchased) return false;

            return !await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public Task<Review?> GetByIdAsync(Guid id)
            => _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);

        public async Task AddAsync(Review review)
            => await _context.Reviews.AddAsync(review);

        public Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync()
            => _context.SaveChangesAsync();
    }
}
