using MediatR;
using System.Collections.Generic;

namespace CatalogService.Api.Queries
{
    public class GetMyProductsQuery : IRequest<List<Result>>
    {
    }
}
