namespace Ecom.Application.CatalogService.Application.DTO
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; }
        public string? BrandName { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<string> Tags { get; set; }
    }
}
