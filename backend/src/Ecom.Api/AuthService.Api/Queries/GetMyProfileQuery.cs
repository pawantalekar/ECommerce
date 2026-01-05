using Ecom.Application.AuthService.Application.DTO;
using MediatR;

namespace AuthService.Api.Queries
{
    public class GetMyProfileQuery : IRequest<UserProfileDto>
    {
        public Guid Id { get; set; }
    }
}
