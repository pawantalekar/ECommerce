namespace Ecom.Application.CatalogService.Application.DTO
{
    public class BrandDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;
    }
}
