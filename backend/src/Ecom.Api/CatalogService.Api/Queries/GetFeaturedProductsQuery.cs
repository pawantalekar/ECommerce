using MediatR;

namespace CatalogService.Api.Queries
{
    public class GetFeaturedProductsQuery : IRequest<List<Result>>
    {
    }
}
