using AutoMapper;
using Ecom.Application.AuthService.Application.DTO;
using Ecom.Application.AuthService.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AuthService.Api.Queries.GetAddress
{
    public class GetAddressesQueryHandler : IRequestHandler<GetAddressesQuery, List<AddressDto>>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetAddressesQueryHandler(IUserRepository userRepository , IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public async Task<List<AddressDto>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not authenticated.");

            var userId = Guid.Parse(userIdClaim);

            var addresses = await _userRepository.GetAddressesAsync(userId, cancellationToken).ConfigureAwait(false);
            var result = _mapper.Map<List<AddressDto>>(addresses);
            return result ?? new List<AddressDto>();
        }
    }
}
