using Ecom.Application.CatalogService.Application.DTO;
using MediatR;

namespace CatalogService.Api.Queries
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ProductForEditDto?>;

}
