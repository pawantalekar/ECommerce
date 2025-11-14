
using MediatR;

namespace CatalogService.Api.Commands.AddProduct
{
    public record AddProductCommand(
         string Name,
         string? ShortDescription,
         string? Description,
         decimal Price,
         string SKU,
         int StockQuantity,
         Guid? CategoryId,
         Guid? BrandId,
         List<string> ImageUrls,
         List<string> Tags
     ) : IRequest<AddProductCommandResult>
    {
        public string? CategoryName { get; init; }
        public string? CategorySlug { get; init; }
        public string? BrandName { get; init; }
        public string? BrandLogoUrl { get; init; }
    }
   }

    
