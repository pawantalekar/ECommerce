using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CatalogService.Api.Commands.DeleteProduct
{
    public class ToggleProductActiveCommandHandler : IRequestHandler<ToggleProductActiveCommand, bool>
    {
        private readonly ICatalogRepository _repo;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ToggleProductActiveCommandHandler(
            ICatalogRepository repo,
            IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(ToggleProductActiveCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) ||
                !Guid.TryParse(userIdClaim, out var sellerId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var product = await _repo.GetByIdAsync(request.ProductId, cancellationToken);

            if (product == null)
                return false;

            if (product.SellerId != sellerId)
                throw new Exception("You can only toggle your own products");

            product.IsActive = !(product.IsActive ?? true);

            await _repo.UpdateProductAsync(product, cancellationToken);

            return product.IsActive.Value;
        }
    }
}
