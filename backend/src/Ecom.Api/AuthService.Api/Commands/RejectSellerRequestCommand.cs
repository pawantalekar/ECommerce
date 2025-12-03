using MediatR;

namespace AuthService.Api.Commands
{
    public record RejectSellerRequestCommand(Guid RequestId, Guid AdminId) : IRequest<Unit>;
}
