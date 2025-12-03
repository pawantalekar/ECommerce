using Ecom.Application.AuthService.Application.DTO;
using MediatR;

namespace AuthService.Api.Queries
{
    public record GetPendingSellerRequestsQuery : IRequest<List<SellerRequestDto>>;
}
