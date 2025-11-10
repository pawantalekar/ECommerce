using Ecom.Application.CatalogService.Application.DTO;
using FluentValidation;

namespace CatalogService.Api.Commands.AddProduct
{
    public class AddProductCommandValidator : AbstractValidator<ProductDto>
    {
        public AddProductCommandValidator() {

            RuleFor(x => x.productId).NotEmpty().WithMessage("Enter a product id ");
            RuleFor(x => x.productName).NotEmpty()
                .WithMessage("ProductName Cant be Empty")
                .MinimumLength(4)
                .WithMessage("Product name should have more than 4 letters");
        }
    }
}
