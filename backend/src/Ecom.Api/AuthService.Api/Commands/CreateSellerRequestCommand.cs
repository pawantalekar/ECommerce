using MediatR;

namespace AuthService.Api.Commands
{
    public record CreateSellerRequestCommand(
        Guid UserId,
        string UserName,
        string UserEmail
    ) : IRequest<Unit>;
}
