using MediatR;

namespace CartService.Api.Queries
{
    public class GetMyCartQuery : IRequest<GetMyCartQueryResult>
    {
    }
}
