using MediatR;

namespace AuthService.Api.Commands
{
    public record RejectReviewCommand(Guid ReviewId, Guid AdminId) : IRequest<Unit>;
}
