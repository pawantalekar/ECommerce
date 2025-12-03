using MediatR;

namespace AuthService.Api.Queries
{
    public record GetCurrentUserRoleQuery : IRequest<string>;
}
