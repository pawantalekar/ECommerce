using FluentValidation;

namespace CartService.Api.Commands.RemoveFromCart
{
    public class RemoveFromCartCommandValidator : AbstractValidator<RemoveFromCartCommand>
    {
        public RemoveFromCartCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
        }
    }
}
