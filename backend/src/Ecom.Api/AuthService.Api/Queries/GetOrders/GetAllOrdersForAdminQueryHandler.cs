using Ecom.Application.AuthService.Application.DTO;
using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Application.OrderService.Application.Interfaces;
using MediatR;

namespace AuthService.Api.Queries.GetOrders
{
    public class GetAllOrdersForAdminQueryHandler : IRequestHandler<GetAllOrdersForAdminQuery, PagedOrdersDto>
    {
        private readonly IOrderRepository orderRepository;

        public GetAllOrdersForAdminQueryHandler(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<PagedOrdersDto> Handle(GetAllOrdersForAdminQuery request, CancellationToken cancellationToken)
        {
            var (orders, total) = await orderRepository.GetAllOrdersForAdminAsync(
                request.OrderStatus,
                request.PaymentStatus,
                request.FromDate,
                request.ToDate,
                request.SearchTerm,
                request.PageNumber,
                request.PageSize,
                cancellationToken
            );

            var orderDtos = orders.Select(static o => new AdminOrderListItemDto(
                o.OrderNumber,
                o.User.Name,
                o.User.Email,
                o.CreatedAt,
                o.TotalAmount,
                o.OrderStatus,
                o.PaymentStatus
            )).ToList();

            return new PagedOrdersDto(orderDtos, total, request.PageNumber, request.PageSize);
        }
    }
}