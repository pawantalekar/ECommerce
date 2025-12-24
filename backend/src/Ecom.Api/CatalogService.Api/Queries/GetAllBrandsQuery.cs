using Ecom.Application.CatalogService.Application.DTO;
using MediatR;

namespace CatalogService.Api.Queries
{
    public class GetAllBrandsQuery : IRequest<List<BrandDto>>
    {
    }
}
