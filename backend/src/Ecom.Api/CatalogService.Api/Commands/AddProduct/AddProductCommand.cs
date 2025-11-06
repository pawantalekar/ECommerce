using Ecom.Application.CatalogService.Application.DTO;
using MediatR;

namespace CatalogService.Api.Commands.AddProduct
{
    public class AddProductCommand : IRequest<AddProductCommandResult>
    {
        public ProductDto Product { get; set; } = new ProductDto();
    }
}
