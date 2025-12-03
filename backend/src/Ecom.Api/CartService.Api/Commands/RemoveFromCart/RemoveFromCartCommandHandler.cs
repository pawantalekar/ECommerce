using Ecom.Application.CartService.Application.DTO;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartService.Api.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, RemoveFromCartCommandResult>
    {
        private readonly ICartRepository _cartRepository;
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RemoveFromCartCommandHandler(
            ICartRepository cartRepository,
            AuthDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RemoveFromCartCommandResult> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartRepository.GetByUserIdWithItemsAsync(userId);

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId)
                       ?? throw new Exception("Item not found in cart");

            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateAsync(cart);

            var cartDto = await BuildCartDto(cart, cancellationToken);

            return new RemoveFromCartCommandResult { Cart = cartDto };
        }

        private Guid GetCurrentUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User
                .Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);

            if (claim == null || !Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("User not authenticated");

            return userId;
        }

        private async Task<CartDto> BuildCartDto(Ecom.Domain.Entities.Cart cart, CancellationToken ct)
        {
            if (!cart.Items.Any())
            {
                return new CartDto { Id = cart.Id, ItemsCount = 0, Total = 0, Items = new() };
            }

            var productIds = cart.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, ct);

            var items = new List<CartItemDto>();

            foreach (var item in cart.Items)
            {
                var product = products[item.ProductId];
                var thumbnail = product.ProductImages.FirstOrDefault(img => img.IsThumbnail == true)?.Url;

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
