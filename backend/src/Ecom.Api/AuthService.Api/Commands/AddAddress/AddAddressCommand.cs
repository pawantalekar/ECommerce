using Ecom.Application.AuthService.Application.DTO;
using MediatR;

namespace AuthService.Api.Commands.AddAddress
{
    public class AddAddressCommand : IRequest<AddressDto>
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string AddressType { get; set; } = string.Empty;
        public bool IsDefault { get; set; } 
    }
}
