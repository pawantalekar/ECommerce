using CatalogService.Api.Commands.UpdateProduct;
using Ecom.Application.CartService.Application.DTO;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartService.Api.Commands.UpdateCart
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly ICatalogRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthDbContext _db;

        public UpdateProductCommandHandler(
            ICatalogRepository repo,
            IHttpContextAccessor httpContextAccessor,
            AuthDbContext db)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var sellerId))
                throw new UnauthorizedAccessException();

            var product = await _repo.GetByIdAsync(request.Id, ct);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            if (product.SellerId != sellerId)
                throw new UnauthorizedAccessException("You can only update your own products");

            product.Name = request.Name;
            product.Slug = request.Name.ToLower().Replace(" ", "-").Replace("--", "-");
            product.ShortDescription = request.ShortDescription;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Sku = request.Sku;
            product.StockQuantity = request.StockQuantity;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            product.IsActive = request.IsActive;
            product.IsFeatured = request.IsFeatured;
           

            _db.ProductImages.RemoveRange(product.ProductImages);
            product.ProductImages = request.ImageUrls.Select((url, index) => new ProductImage
            {
                Id = Guid.NewGuid(),
                Url = url,
                SortOrder = index,
                IsThumbnail = index == 0
            }).ToList();

            var incomingTags = request.Tags ?? new List<string>();
            var normalized = incomingTags.Select(t => t.Trim().ToLowerInvariant()).Distinct().ToList();
            var existingTags = await _db.Tags.Where(t => normalized.Contains(t.Name.ToLowerInvariant())).ToListAsync(ct);
            var tagDict = existingTags.ToDictionary(t => t.Name.ToLowerInvariant());

            foreach (var name in normalized.Where(n => !tagDict.ContainsKey(n)))
            {
                var newTag = new Tag { Id = Guid.NewGuid(), Name = name };
                _db.Tags.Add(newTag);
                tagDict[name] = newTag;
            }

            product.ProductTags = tagDict.Values.Select(tag => new ProductTag { Tag = tag }).ToList();

            await _repo.UpdateProductAsync(product, ct);

            return Unit.Value;
        }
    }
}
