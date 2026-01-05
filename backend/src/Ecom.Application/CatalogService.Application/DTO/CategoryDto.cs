namespace Ecom.Application.CatalogService.Application.DTO
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public bool? IsActive { get; set; }

    }
}
