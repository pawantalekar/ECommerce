using MediatR;

namespace CatalogService.Api.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        string? ShortDescription,
        string? Description,
        decimal Price,
        string Sku,
        int StockQuantity,
        Guid CategoryId,
        Guid? BrandId,
        string? BrandLogoUrl,
        bool IsActive,
        bool IsFeatured,
        List<string> ImageUrls,
        List<string> Tags
    ) : IRequest<Unit>;

}
