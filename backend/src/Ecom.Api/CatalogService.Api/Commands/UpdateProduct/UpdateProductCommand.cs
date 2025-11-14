using MediatR;

namespace CatalogService.Api.Commands.UpdateProduct
{
    public record UpdateProductCommand(
        Guid Id,
        string Name,
        string? ShortDescription,
        string? Description,
        decimal Price,
        string SKU,
        int StockQuantity,
        Guid? CategoryId,
        Guid? BrandId,
        List<string> ImageUrls,
        List<string> Tags,
        bool IsActive,
        bool IsFeatured
    ) : IRequest<UpdateProductCommandResult>
    {
        public string? CategoryName { get; init; }
        public string? CategorySlug { get; init; }
        public string? BrandName { get; init; }
        public string? BrandLogoUrl { get; init; }
    }
}
