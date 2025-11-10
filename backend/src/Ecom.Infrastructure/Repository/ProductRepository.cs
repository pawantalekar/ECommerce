using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Infrastructure.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AuthDbContext context;

        public ProductRepository(AuthDbContext context)
        {
            this.context = context;
        }
        public async Task<Product> CreateProductAsync(Product product, List<int> categoryIds)
        {
            await context.Products.AddAsync(product);

            return product;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
          var result= await context.Products.Include(p => p.Categories).ToListAsync();
            return result;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await context.Products.Include(p => p.Categories).FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<Product?> UpdateProductAsync(int id, Product product, List<int> categoryIds)
        {
            throw new NotImplementedException();
        }
    }
}
