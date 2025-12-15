using Ecom.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
