using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CatalogService.Api.Commands.UpdateProduct
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductCommandResult>
    {
        private readonly ICatalogRepository _repository;

        public UpdateProductCommandHandler(ICatalogRepository repository)
        {
            _repository = repository;
        }

        public async Task<UpdateProductCommandResult> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
                throw new ArgumentException("Product not found");

            product.Name = request.Name;
            product.Slug = (request.Name ?? Guid.NewGuid().ToString())
                .ToLower().Replace(" ", "-").Replace("--", "-");
            product.ShortDescription = request.ShortDescription;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Sku = request.SKU;
            product.StockQuantity = request.StockQuantity;

            // Category
            if (request.CategoryId.HasValue && request.CategoryId.Value != Guid.Empty)
                product.CategoryId = request.CategoryId.Value;
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

            // Brand
            if (request.BrandId.HasValue && request.BrandId.Value != Guid.Empty)
                product.BrandId = request.BrandId.Value;
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

            // Images
            product.ProductImages.Clear();
            if (request.ImageUrls != null && request.ImageUrls.Count > 0)
            {
                product.ProductImages = request.ImageUrls.Select((url, i) => new ProductImage
                {
                    Id = Guid.NewGuid(),
                    Url = url,
                    AltText = $"{request.Name} by {request.BrandName ?? "Unknown Brand"} - {request.CategoryName ?? "Product"} view {i + 1}",
                    SortOrder = i,
                    IsThumbnail = i == 0
                }).ToList();
            }

            // Tags
            var tags = (request.Tags ?? new List<string>()).Select(tagName => new ProductTag
            {
                Tag = new Tag { Name = tagName }
            }).ToList();
            product.ProductTags.Clear();
            product.ProductTags = tags;

            await _repository.UpdateProductAsync(product, cancellationToken);
            return new UpdateProductCommandResult(product.Id, product.Slug);
        }
    }
}
