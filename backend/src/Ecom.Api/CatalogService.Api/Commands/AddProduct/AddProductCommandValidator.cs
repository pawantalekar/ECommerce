
using Ecom.Application.CatalogService.Application.Interfaces;
using FluentValidation;

namespace CatalogService.Api.Commands.AddProduct
{
    public class AddProductCommandValidator : AbstractValidator<AddProductCommand>
    {
        public AddProductCommandValidator(ICatalogRepository repository)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.SKU).NotEmpty().MaximumLength(100);
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0);
            //RuleFor(x => x.CategoryId).NotEmpty();
            //RuleFor(x => x.ImageUrls).NotNull();
            RuleFor(x => x.Tags).NotNull();
        }
    }
}
