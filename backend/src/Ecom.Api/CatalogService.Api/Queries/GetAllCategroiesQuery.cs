using Ecom.Application.CatalogService.Application.DTO;
using Ecom.Domain.Entities;
using MediatR;

namespace CatalogService.Api.Queries
{
    public class GetAllCategroiesQuery : IRequest<List<CategoryDto>>
    {
    }
}
