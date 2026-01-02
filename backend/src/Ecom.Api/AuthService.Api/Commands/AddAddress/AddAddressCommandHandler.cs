using AutoMapper;
using Ecom.Application.AuthService.Application.DTO;
using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AuthService.Api.Commands.AddAddress
{
    public class AddAddressCommandHandler : IRequestHandler<AddAddressCommand, AddressDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper mapper;

        public AddAddressCommandHandler(
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        public async Task<AddressDto> Handle(AddAddressCommand request, CancellationToken ct)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated.");

            var userId = Guid.Parse(userIdClaim);

            var address = new Address
            {
                Id = Guid.NewGuid(),
                UserProfileId = userId,
                FullName = request.FullName,
                Phone = request.Phone,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                Pincode = request.Pincode,
                Country = request.Country,
                AddressType = request.AddressType,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.UtcNow
            };
            //var address = mapper.Map<Address>(request);
            var addedAddress = await _userRepository.AddAddressAsync(userId, address, ct);

            if (addedAddress == null)
                throw new InvalidOperationException("Failed to add address.");

            return new AddressDto
            {
                Id = addedAddress.Id,
                FullName = addedAddress.FullName,
                Phone = addedAddress.Phone,
                AddressLine1 = addedAddress.AddressLine1,
                AddressLine2 = addedAddress.AddressLine2,
                City = addedAddress.City,
                State = addedAddress.State,
                Pincode = addedAddress.Pincode,
                Country = addedAddress.Country,
                AddressType = addedAddress.AddressType,
                IsDefault = addedAddress.IsDefault
            };
        }
    }
}