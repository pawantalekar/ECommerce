using Ecom.Application.CatalogService.Application.DTO;
using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Queries
{
    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, SearchProductsResult>
    {
        private readonly ICatalogRepository _repo;

        public SearchProductsQueryHandler(ICatalogRepository repo) => _repo = repo;

        public async Task<SearchProductsResult> Handle(SearchProductsQuery request, CancellationToken ct)
        {
            var search = request.Query?.Trim().ToLower() ?? "";

            var query = _repo.GetQueryable()
                .Where(p => p.IsActive == true && p.StockQuantity > 0);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    (p.ShortDescription ?? "").ToLower().Contains(search) ||
                    (p.Description ?? "").ToLower().Contains(search) ||
                    (p.Brand != null && p.Brand.Name.ToLower().Contains(search)) ||
                    (p.Category != null && p.Category.Name.ToLower().Contains(search)) ||
                    p.ProductTags.Any(pt => pt.Tag != null && pt.Tag.Name.ToLower().Contains(search))
                );
            }

            if (request.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == request.CategoryId.Value);

            if (request.BrandId.HasValue)
                query = query.Where(p => p.BrandId == request.BrandId.Value);

            var total = await query.CountAsync(ct);

            var products = await query
                .OrderByDescending(p => p.Name.ToLower().StartsWith(search) ? 1 : 0)
                .ThenBy(p => p.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductSearchResultDto(
                    p.Slug,
                    p.Name,
                    p.ShortDescription ?? "",
                    p.Price,
                    p.ProductImages.OrderBy(i => i.SortOrder).Select(i => i.Url).FirstOrDefault() ?? "",
                    p.Brand != null ? p.Brand.Name : "",
                    p.IsFeatured ?? false,
                    p.ProductTags.Select(pt => pt.Tag != null ? pt.Tag.Name : "").ToList()
                ))
                .ToListAsync(ct);

            return new SearchProductsResult(products, total);
        }
    }
}