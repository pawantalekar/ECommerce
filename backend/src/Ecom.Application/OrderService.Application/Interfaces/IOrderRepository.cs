using Ecom.Domain.Entities;
using Ecom.Domain.Enums;

namespace Ecom.Application.OrderService.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<(List<Order> Orders, int Total)> GetAllOrdersForAdminAsync(string[]? orderStatus, string[]? paymentStatus, DateTime? fromDate, DateTime? toDate, string? searchTerm, int pageNumber, int pageSize, CancellationToken ct);

        Task<Order?> GetOrderByNumberAsync(string orderNumber, CancellationToken ct);

        Task UpdateOrderStatusAsync(Order order, OrderStatusEnum newStatus, Guid? adminUserId, string? notes, CancellationToken ct);
    }
}
