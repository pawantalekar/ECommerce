using MediatR;

namespace CatalogService.Api.Commands.DeleteProduct
{
    public record ToggleProductActiveCommand(Guid ProductId) : IRequest<bool>;
}
