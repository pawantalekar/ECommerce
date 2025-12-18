using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.CatalogService.Application.DTO
{
    public class ProductForEditDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public List<ProductImageDto> Images { get; set; } = new();
        public List<string> Tags { get; set; } = new();
    }

    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int SortOrder { get; set; }
        public bool IsThumbnail { get; set; }
    }
}
