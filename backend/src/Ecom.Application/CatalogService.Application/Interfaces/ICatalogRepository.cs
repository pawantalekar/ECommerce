using Ecom.Domain.Entities;
using Product = Ecom.Domain.Entities.Product;

namespace Ecom.Application.CatalogService.Application.Interfaces
{
    public interface ICatalogRepository
    {
        Task AddProductAsync(Product product, CancellationToken ct);
        Task<Product?> GetBySlugAsync(string slug, CancellationToken ct);
        Task<List<Product>> GetAllProductsAsync(CancellationToken ct);
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);

        Task UpdateProductAsync(Product product, CancellationToken ct);
        IQueryable<Product> GetQueryable();
        Task<List<Product>> GetProductsBySellerIdAsync(Guid sellerId, CancellationToken ct);
        Task UpdateFullProductAsync(Product updatedProduct, List<string> imageUrls, List<string> tagNames, CancellationToken ct);
        Task<List<Product>> GetFeaturedProductsAsync(CancellationToken ct);
        Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct);
        Task<List<Brand>> GetAllBrandsAsync(CancellationToken ct);
        Task<List<Product>> GetProductsByCategoryIdAsync(Guid Id, CancellationToken ct);
    }
}
