using Ecom.Application.CatalogService.Application.DTO;

namespace CatalogService.Api.Commands.AddProduct
{
    public class AddProductCommandResult
    {
        public ProductDto Product { get; set; } = new ProductDto();
        public string message { get; set; } = string.Empty;
    }
}
