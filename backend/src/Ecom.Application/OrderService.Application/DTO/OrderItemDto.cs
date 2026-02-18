using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.OrderService.Application.DTO
{
    public record OrderItemDto(string ProductId, string ProductName, string? ThumbnailUrl, decimal UnitPrice, int Quantity, string Slug);
}
