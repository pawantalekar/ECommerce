using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Api.Queries
{
    public record GetAllProductsQuery() : IRequest<List<Result>>;
}
