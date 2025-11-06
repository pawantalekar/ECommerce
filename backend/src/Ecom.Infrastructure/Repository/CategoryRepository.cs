using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Domain.Entities;
using System.Data.Entity;

namespace Ecom.Infrastructure.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AuthDbContext context;
        public CategoryRepository(AuthDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = await context.Categories.ToListAsync();
            return categories;
        }
    }
}
