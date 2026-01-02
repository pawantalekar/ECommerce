    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;
    using Ecom.Application.AuthService.Application.Interfaces;

    namespace AuthService.Api.Commands.UpdateAddress
    {
        public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, bool>
        {
            private readonly IUserRepository _userRepository;

            public UpdateAddressCommandHandler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<bool> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
            {
                return await _userRepository.UpdateAddressAsync(
                    request.Id,
                    request.FullName,
                    request.Phone,
                    request.AddressLine1,
                    request.AddressLine2,
                    request.City,
                    request.State,
                    request.Pincode,
                    request.Country,
                    request.AddressType,
                    request.IsDefault,
                    cancellationToken
                );
            }
        }
    }

