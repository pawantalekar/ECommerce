using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;

namespace CatalogService.Api.Queries
{
    public class GetFeaturedProductsQueryHandler : IRequestHandler<GetFeaturedProductsQuery, List<Result>>
    {
        private readonly ICatalogRepository _repo;

        public GetFeaturedProductsQueryHandler(ICatalogRepository repo) => _repo = repo;

        public async Task<List<Result>> Handle(GetFeaturedProductsQuery request, CancellationToken ct)
        {
            var products = await _repo.GetFeaturedProductsAsync(ct);

            return products.Select(product => new Result(
                product.Id,
                product.Name,
                product.Slug,
                product.ShortDescription,
                product.Description,
                product.Price,
                product.Sku,
                product.StockQuantity,
                product.Category.Name,
                product.Brand?.Name,
                product.IsActive != false,
                product.IsFeatured != false,
                product.ProductImages.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
                product.ProductTags.Select(pt => pt.Tag.Name).ToList()
            )).ToList();
        }
    }
}
