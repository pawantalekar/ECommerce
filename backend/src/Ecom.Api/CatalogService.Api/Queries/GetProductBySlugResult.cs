using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Api.Queries
{
    public record Result(
         Guid Id,
         string Name,
         string Slug,
         string? ShortDescription,
         string? Description,
         decimal Price,
         string Sku,
         int StockQuantity,
         string CategoryName,
         string? BrandName,
         bool IsActive,
         bool IsFeatured,
         List<string> ImageUrls,
         List<string> Tags
     );
}
