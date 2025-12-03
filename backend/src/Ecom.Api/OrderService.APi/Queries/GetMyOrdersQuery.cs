using Ecom.Application.OrderService.Application.DTO;
using MediatR;

namespace OrderService.APi.Queries
{
    public record GetMyOrdersQuery : IRequest<List<OrderDto>>;
}
