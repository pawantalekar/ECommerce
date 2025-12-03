using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.OrderService.Application.DTO
{
    public record CheckoutItemDto(Guid ProductId, int Quantity);
}
