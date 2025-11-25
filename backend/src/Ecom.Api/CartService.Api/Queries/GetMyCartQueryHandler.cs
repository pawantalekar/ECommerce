using Ecom.Application.CartService.Application.DTO;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CartService.Api.Queries
{
    public class GetMyCartQueryHandler : IRequestHandler<GetMyCartQuery, GetMyCartQueryResult>
    {
        private readonly ICartRepository _cartRepository;
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyCartQueryHandler(
            ICartRepository cartRepository,
            AuthDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _cartRepository = cartRepository;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetMyCartQueryResult> Handle(GetMyCartQuery request, CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            var cart = await _cartRepository.GetByUserIdWithItemsAsync(userId);

            var cartDto = await BuildCartDto(cart, cancellationToken);

            return new GetMyCartQueryResult { Cart = cartDto };
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

        private async Task<CartDto> BuildCartDto(Ecom.Domain.Entities.Cart cart, CancellationToken ct)
        {
            if (!cart.Items.Any())
            {
                return new CartDto
                {
                    Id = cart.Id,
                    ItemsCount = 0,
                    Total = 0,
                    Items = new()
                };
            }

            var productIds = cart.Items.Select(i => i.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .Include(p => p.ProductImages)
                .ToDictionaryAsync(p => p.Id, ct);

            var items = new List<CartItemDto>();

            foreach (var item in cart.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    continue;

                var thumbnail = product.ProductImages
                    .FirstOrDefault(img => img.IsThumbnail == true)?.Url
                    ?? product.ProductImages.FirstOrDefault()?.Url;

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
