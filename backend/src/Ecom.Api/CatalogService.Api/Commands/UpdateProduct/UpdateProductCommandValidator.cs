using CatalogService.Api.Helpers;
using Ecom.Application.CatalogService.Application.Interfaces;
using FluentValidation;

namespace CatalogService.Api.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator(ICatalogRepository repository)
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Sku).NotEmpty().MaximumLength(100);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Tags).NotNull();

            RuleFor(x => x.Price)
                .MustAsync(async (price, cancellation) => await ProductHelper.ValidateProductPrice(price))
                .WithMessage("Invalid product price.");
        }
    }
}
