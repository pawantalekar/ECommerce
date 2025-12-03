using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.CartService.Application.DTO
{
    public class CartDto
    {
        public Guid Id { get; set; }
        public int ItemsCount { get; set; }
        public decimal Total { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }
}
