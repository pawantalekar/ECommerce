namespace Ecom.Application.AuthService.Application.DTO
{
    public class UserProfileDto
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string? Gender { get; set; }

        public string MobileNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
