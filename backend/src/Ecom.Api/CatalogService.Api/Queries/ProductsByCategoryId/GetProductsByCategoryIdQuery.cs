using Ecom.Application.CatalogService.Application.DTO;
using MediatR;

namespace CatalogService.Api.Queries.ProductsByCategoryId
{
    public class GetProductsByCategoryIdQuery : IRequest<List<ProductDTO>>
    {
        public Guid Id { get; }

        public GetProductsByCategoryIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
