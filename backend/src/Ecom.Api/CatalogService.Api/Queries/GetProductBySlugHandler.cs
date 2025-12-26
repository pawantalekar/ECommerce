using MediatR;
using Ecom.Application.CatalogService.Application.Interfaces;

namespace CatalogService.Api.Queries
{
    public class GetProductBySlugHandler : IRequestHandler<GetProductBySlugQuery, Result?>
    {
        private readonly ICatalogRepository repo;
        public GetProductBySlugHandler(ICatalogRepository repo) => this.repo = repo;

        public async Task<Result?> Handle(GetProductBySlugQuery request, CancellationToken ct)
        {
            var product = await repo.GetBySlugAsync(request.Slug, ct);
            if (product == null) return null;

            return new Result(
                product.Id,
                product.Name,
                product.Slug,
                product.ShortDescription,
                product.Description,
                product.Price,
                product.Sku,
                product.StockQuantity,
                product.CategoryId,
                product.Category.Name,
                product.Brand?.Name,
                product.IsActive ?? true,
                product.IsFeatured ?? false,
                product.ProductImages.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
                product.ProductTags.Select(pt => pt.Tag.Name).ToList()
            );
        }
    }
}