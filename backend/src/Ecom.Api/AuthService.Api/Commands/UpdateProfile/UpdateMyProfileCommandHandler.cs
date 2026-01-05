using AutoMapper;
using Ecom.Application.AuthService.Application.DTO;
using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AuthService.Api.Commands.UpdateProfile
{
    public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, UpdateProfileDto>
    {
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        private readonly IMapper mapper;
        public UpdateMyProfileCommandHandler(AuthDbContext context, IHttpContextAccessor httpContextAccessor,IUserRepository userRepository, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            this.mapper = mapper;
        }
        public async Task<UpdateProfileDto> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User is not authenticated.");

            var updatedProfile = await _userRepository.UpdateProfileAsync(userId, request.FirstName, request.LastName, request.Gender, request.MobileNumber, cancellationToken);

            if (updatedProfile == null)
            {
                throw new InvalidOperationException("User or profile not found.");
            }

            var updateProfileDto = mapper.Map<UpdateProfileDto>(updatedProfile);

            // Return DTO
            return updateProfileDto;
        }
    }
}
