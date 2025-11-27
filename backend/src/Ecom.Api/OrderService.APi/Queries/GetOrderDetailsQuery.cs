using Ecom.Application.OrderService.Application.DTO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.APi.Queries
{
    public record GetOrderDetailsQuery(Guid OrderId) : IRequest<OrderDto>;
}
