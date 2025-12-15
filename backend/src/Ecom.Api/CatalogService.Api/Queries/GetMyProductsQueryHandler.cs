using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CatalogService.Api.Queries
{
    public class GetMyProductsQueryHandler : IRequestHandler<GetMyProductsQuery, List<Result>>
    {
        private readonly ICatalogRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyProductsQueryHandler(
            ICatalogRepository repo,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Result>> Handle(GetMyProductsQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var sellerId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var products = await _repo.GetProductsBySellerIdAsync(sellerId, cancellationToken);

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
                product.IsActive ?? true,
                product.IsFeatured ?? false,
                product.ProductImages.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList(),
                product.ProductTags.Select(pt => pt.Tag.Name).ToList()
            )).ToList();
        }
    }
}