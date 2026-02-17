using Ecom.Application.OrderService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AuthDbContext _db;
        public OrderRepository(AuthDbContext db)
        {
            _db = db;
        }

        public async Task<(List<Order> Orders, int Total)> GetAllOrdersForAdminAsync(
          string[]? orderStatus,
          string[]? paymentStatus,
          DateTime? fromDate,
          DateTime? toDate,
          string? searchTerm,
          int pageNumber,
          int pageSize,
          CancellationToken ct)
        {
            var query = _db.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .AsNoTracking()
                .AsQueryable();

            if (orderStatus != null && orderStatus.Length > 0)
            {
                var statusEnums = orderStatus
                    .Where(s => Enum.TryParse<OrderStatusEnum>(s, out _))
                    .Select(s => Enum.Parse<OrderStatusEnum>(s))
                    .ToList();
                query = query.Where(o => statusEnums.Contains(o.OrderStatus));
            }

            if (paymentStatus != null && paymentStatus.Length > 0)
            {
                var statusEnums = paymentStatus
                    .Where(s => Enum.TryParse<PaymentStatusEnum>(s, out _))
                    .Select(s => Enum.Parse<PaymentStatusEnum>(s))
                    .ToList();
                query = query.Where(o => statusEnums.Contains(o.PaymentStatus));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                var endOfDay = toDate.Value.Date.AddDays(1);
                query = query.Where(o => o.CreatedAt < endOfDay);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(o =>
                    o.OrderNumber.Contains(searchTerm) ||
                    o.User.Name.Contains(searchTerm) ||
                    o.User.Email.Contains(searchTerm)
                );
            }

            var total = await query.CountAsync(ct);

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (orders, total);
        }

        public async Task<Order?> GetOrderByNumberAsync(string orderNumber, CancellationToken ct)
        {
            return await _db.Orders
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, ct);
        }

        public async Task UpdateOrderStatusAsync(Order order, OrderStatusEnum newStatus, Guid? adminUserId, string? notes, CancellationToken ct)
        {
            var oldStatus = order.OrderStatus;
            order.OrderStatus = newStatus;

            var statusHistory = new OrderStatusHistory
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Status = newStatus,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = adminUserId,
                Notes = notes ?? $"Status changed from {oldStatus} to {newStatus}"
            };

            _db.OrderStatusHistories.Add(statusHistory);
            await _db.SaveChangesAsync(ct);
        }
    }
}
