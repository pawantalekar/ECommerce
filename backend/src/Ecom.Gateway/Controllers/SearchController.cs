using CatalogService.Api.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecom.Gateway.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<SearchProductsResult>> Search([FromQuery] SearchProductsQuery query)
        => Ok(await _mediator.Send(query));
}
