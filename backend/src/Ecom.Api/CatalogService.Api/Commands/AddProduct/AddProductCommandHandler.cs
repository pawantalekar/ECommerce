using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.Api.Commands.AddProduct
{
    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, AddProductCommandResult>
    {
        private readonly ICatalogRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddProductCommandHandler(ICatalogRepository repository, IHttpContextAccessor httpContextAccessor)
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AddProductCommandResult> Handle(AddProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                SellerId = GetCurrentUserId(),
                Name = request.Name,
                Slug = (request.Name ?? Guid.NewGuid().ToString()).ToLower().Replace(" ", "-").Replace("--", "-"),
                ShortDescription = request.ShortDescription,
                Description = request.Description,
                Price = request.Price,
                Sku = request.SKU,
                StockQuantity = request.StockQuantity,
                IsActive = true,
                IsFeatured = false,
                CreatedAt = DateTime.UtcNow
            };

            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
            {
                product.CategoryId = request.CategoryId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(request.CategoryName))
            {
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = request.CategoryName.Trim(),
                    Slug = string.IsNullOrWhiteSpace(request.CategorySlug)
                        ? request.CategoryName.Trim().ToLower().Replace(" ", "-")
                        : request.CategorySlug.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                product.Category = category;
                product.CategoryId = category.Id;
            }

            if (request.BrandId.HasValue && request.BrandId.Value != Guid.Empty)
            {
                product.BrandId = request.BrandId.Value;
            }
            else if (!string.IsNullOrWhiteSpace(request.BrandName))
            {
                var brand = new Brand
                {
                    Id = Guid.NewGuid(),
                    Name = request.BrandName.Trim(),
                    Slug = request.BrandName.Trim().ToLower().Replace(" ", "-"),
                    LogoUrl = string.IsNullOrWhiteSpace(request.BrandLogoUrl) ? null : request.BrandLogoUrl.Trim()
                };
                product.Brand = brand;
                product.BrandId = brand.Id;
            }

            var images = new List<ProductImage>();
            if (request.ImageUrls != null && request.ImageUrls.Count > 0)
            {
                images = request.ImageUrls.Select((url, i) => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    Url = url,
                    AltText = $"{request.Name} by {request.BrandName ?? "Unknown Brand"} - {request.CategoryName ?? "Product"} view {i + 1}",
                    SortOrder = i,
                    IsThumbnail = i == 0
                }).ToList();
            }
            product.ProductImages = images;


            product.ProductTags = (request.Tags ?? new List<string>()).Select(tagName => new ProductTag
            {
                Tag = new Tag { Name = tagName }
            }).ToList();

            await _repository.AddProductAsync(product, cancellationToken);
            return new AddProductCommandResult(product.Id, product.Slug);
        }
        private Guid GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
                throw new UnauthorizedAccessException("User not authenticated");
            return id;
        }
    }
}
