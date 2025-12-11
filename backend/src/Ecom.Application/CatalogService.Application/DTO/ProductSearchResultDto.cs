using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.CatalogService.Application.DTO
{
    public record ProductSearchResultDto(
    string Slug,
    string Name,
    string ShortDescription,
    decimal Price,
    string ImageUrl,
    string BrandName,
    bool IsFeatured,
    IReadOnlyList<string> Tags
);
}
