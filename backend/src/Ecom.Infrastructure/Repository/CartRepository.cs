using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Infrastructure.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly AuthDbContext _context;

        public CartRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<Cart?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> GetByUserIdWithItemsAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Items = new List<CartItem>()
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }



        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Entry(cart).State = EntityState.Modified;

            foreach (var item in cart.Items)
            {
                if (item.Id == Guid.Empty)
                    item.Id = Guid.NewGuid();

                var exists = await _context.CartItems.AnyAsync(x => x.Id == item.Id);

                _context.Entry(item).State = exists ? EntityState.Modified : EntityState.Added;
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}
