using Ecom.Application.CartService.Application.DTO;

namespace CartService.Api.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandResult
    {
        public CartDto Cart { get; set; } = null!;
    }
}
