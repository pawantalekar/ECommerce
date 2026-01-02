using MediatR;

namespace AuthService.Api.Commands.DeleteAddress
{
    public class DeleteAddressCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public DeleteAddressCommand(Guid id)
        {
            this.Id = id;
        }
    }
}
