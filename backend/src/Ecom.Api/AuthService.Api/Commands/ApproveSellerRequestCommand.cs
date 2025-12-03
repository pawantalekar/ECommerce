using MediatR;

namespace AuthService.Api.Commands
{
    public record ApproveSellerRequestCommand(Guid RequestId, Guid AdminId) : IRequest<Unit>;
}
