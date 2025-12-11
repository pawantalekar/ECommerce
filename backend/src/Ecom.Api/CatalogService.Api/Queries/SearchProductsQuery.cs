using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Api.Queries
{
    public record SearchProductsQuery(
    [FromQuery(Name = "q")]
    string Query,
    Guid? CategoryId = null,
    Guid? BrandId = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<SearchProductsResult>;
}
