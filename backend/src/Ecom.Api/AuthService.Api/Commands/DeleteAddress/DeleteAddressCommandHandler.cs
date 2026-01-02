using Ecom.Application.AuthService.Application.Interfaces;
using MediatR;

namespace AuthService.Api.Commands.DeleteAddress
{
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public DeleteAddressCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            return await _userRepository.DeleteAddressAsync(request.Id, cancellationToken);
        }
    }
}
