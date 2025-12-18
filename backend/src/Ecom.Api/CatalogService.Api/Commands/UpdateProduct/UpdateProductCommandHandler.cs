using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CatalogService.Api.Commands.UpdateProduct
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

            // Update scalar properties
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

            if (request.BrandId.HasValue)
            {
                var brand = await _db.Brands.FirstOrDefaultAsync(b => b.Id == request.BrandId.Value, ct);
                if (brand != null && !string.IsNullOrWhiteSpace(request.BrandLogoUrl))
                {
                    brand.LogoUrl = request.BrandLogoUrl;
                    _db.Brands.Update(brand);
                }
            }

            // Pass images and tags to the repository
            await _repo.UpdateFullProductAsync(
                product,
                request.ImageUrls ?? new List<string>(),
                request.Tags ?? new List<string>(),
                ct);

            return Unit.Value;
        }
    }
}
