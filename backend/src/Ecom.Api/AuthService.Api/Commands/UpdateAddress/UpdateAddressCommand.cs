using MediatR;

namespace AuthService.Api.Commands.UpdateAddress
{
    public class UpdateAddressCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Country { get; set; }
        public string AddressType { get; set; }
        public bool IsDefault { get; set; }
    }
}
