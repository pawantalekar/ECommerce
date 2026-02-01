using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.OrderService.Application.DTO
{
    public record OrderDto(
        Guid Id,
        string OrderNumber,
        decimal TotalAmount,
        string PaymentStatus,
        string OrderStatus,
        DateTime CreatedAt,
        List<OrderItemDto> Items,
        ShippingAddressDto ShippingAddress,
        List<OrderStatusHistoryDto>? StatusHistory = null
    );

    public record OrderStatusHistoryDto(
        string Status,
        DateTime ChangedAt,
        string? Notes
    );
}
