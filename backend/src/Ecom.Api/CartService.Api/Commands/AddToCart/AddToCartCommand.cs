using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Api.Commands.AddToCart
{
    public class AddToCartCommand : IRequest<AddToCartCommandResult>
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
