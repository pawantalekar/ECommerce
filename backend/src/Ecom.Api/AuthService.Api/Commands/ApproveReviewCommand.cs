using MediatR;

namespace AuthService.Api.Commands
{
    public record ApproveReviewCommand(Guid ReviewId, Guid AdminId) : IRequest<Unit>;
}
