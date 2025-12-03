using Ecom.Application.CartService.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Api.Commands.AddToCart
{
    public class AddToCartCommandResult
    {
        public CartDto Cart { get; set; } = null!;
    }
}
