using CartService.Api.Commands.AddToCart;
using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Application.CartService.Application.DTO;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartService.Api.Commands.AddToCart
{
    public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, AddToCartCommandResult>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddToCartCommandHandler(
            ICartRepository cartRepository,
            ICatalogRepository catalogRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _catalogRepository = catalogRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AddToCartCommandResult> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            var cart = await _cartRepository.GetByUserIdWithItemsAsync(userId);

            var product = await _catalogRepository.GetByIdAsync(request.ProductId, cancellationToken);

            if (product == null)
                throw new Exception("Product not found or inactive");

            if (product.StockQuantity < request.Quantity)
                throw new Exception("Insufficient stock");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(cart);

            var cartDto = await BuildCartDto(cart, cancellationToken);

            return new AddToCartCommandResult { Cart = cartDto };
        }

        private Guid GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                throw new UnauthorizedAccessException("No HTTP context.");

            var claim = httpContext.User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            );

            if (claim == null)
                throw new UnauthorizedAccessException("User not authenticated");

            if (!Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID format");

            return userId;
        }

        private async Task<CartDto> BuildCartDto(Cart cart, CancellationToken ct)
        {
            var items = new List<CartItemDto>();

            foreach (var item in cart.Items)
            {
                var product = await _catalogRepository.GetByIdAsync(item.ProductId, ct);

                if (product == null)
                    continue;

                var thumbnail = product.ProductImages
                    .FirstOrDefault(img => img.IsThumbnail == true)?.Url;

                items.Add(new CartItemDto
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Slug = product.Slug,
                    Price = product.Price,
                    ThumbnailUrl = thumbnail,
                    Quantity = item.Quantity
                });
            }

            return new CartDto
            {
                Id = cart.Id,
                ItemsCount = items.Sum(i => i.Quantity),
                Total = items.Sum(i => i.SubTotal),
                Items = items
            };
        }
    }
}
