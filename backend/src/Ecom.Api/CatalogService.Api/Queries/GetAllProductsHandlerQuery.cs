using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Api.Queries
{
    public class GetAllProductsHandlerQuery : IRequestHandler<GetAllProductsQuery, List<Result>>
    {
        private readonly ICatalogRepository _repo;

        public GetAllProductsHandlerQuery(ICatalogRepository repo) => _repo = repo;

        public async Task<List<Result>> Handle(GetAllProductsQuery request, CancellationToken ct)
        {
            var products = await _repo.GetAllProductsAsync(ct);

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
                product.IsFeatured ?? false,
                product.ProductImages.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
                product.ProductTags.Select(pt => pt.Tag.Name).ToList()
            )).ToList();
        }
    }
}
