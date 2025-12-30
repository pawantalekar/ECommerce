namespace Ecom.Application.AuthService.Application.DTO
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Gender { get; set; }

        public string MobileNumber { get; set; } = null!;
       
    }
}
