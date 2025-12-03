using Ecom.Application.AuthService.Application.DTO;
using MediatR;

namespace AuthService.Api.Commands
{
    public record RequestSellerRoleCommand : IRequest<CreateSellerRequestResponseDto>;
}
