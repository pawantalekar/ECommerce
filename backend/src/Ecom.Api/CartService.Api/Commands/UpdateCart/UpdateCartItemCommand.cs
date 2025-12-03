using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Api.Commands.UpdateCart
{
    public class UpdateCartItemCommand : IRequest<UpdateCartItemCommandResult>
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
