using Ecom.Application.CatalogService.Application.DTO;
using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CatalogService.Api.Queries
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductForEditDto?>
    {
        private readonly ICatalogRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetProductByIdQueryHandler(ICatalogRepository repo, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ProductForEditDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var sellerId))
                return null;

            var product = await _repo.GetByIdAsync(request.Id, cancellationToken);

            if (product == null || product.SellerId != sellerId)
                return null;

            return new ProductForEditDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Price = product.Price,
                Sku = product.Sku,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                BrandId = product.BrandId,
                BrandName = product.Brand?.Name,
                IsActive = product.IsActive ?? true,
                IsFeatured = product.IsFeatured ?? false,
                Images = product.ProductImages
                    .OrderBy(i => i.SortOrder)
                    .Select(i => new ProductImageDto
                    {
                        Id = i.Id,
                        Url = i.Url,
                        SortOrder = (int)i.SortOrder,
                        IsThumbnail = (bool)i.IsThumbnail
                    }).ToList(),
                Tags = product.ProductTags.Select(pt => pt.Tag.Name).ToList()
            };
        }
    }
}
