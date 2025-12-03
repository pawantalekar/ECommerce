using MediatR;

namespace CartService.Api.Commands.RemoveFromCart
{
    public class RemoveFromCartCommand : IRequest<RemoveFromCartCommandResult>
    {
        public Guid ProductId { get; set; }
    }
}
