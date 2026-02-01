using Ecom.Application.AuthService.Application.DTO;
using MediatR;

namespace AuthService.Api.Queries.GetOrders
{
    public record GetAllOrdersForAdminQuery(
    string[]? OrderStatus = null,
    string[]? PaymentStatus = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? SearchTerm = null,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<PagedOrdersDto>;
}
