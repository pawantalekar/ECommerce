using Ecom.Application.AuthService.Application.DTO;
using MediatR;

namespace AuthService.Api.Commands.UpdateProfile
{
    public class UpdateMyProfileCommand : IRequest<UpdateProfileDto>
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Gender { get; set; }

        public string MobileNumber { get; set; } = null!;
       
    }
}
