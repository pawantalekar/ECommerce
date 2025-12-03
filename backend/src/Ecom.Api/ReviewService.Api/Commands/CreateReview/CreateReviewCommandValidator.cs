using FluentValidation;

namespace ReviewService.Api.Commands.CreateReview
{
    public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewCommandValidator()
        {
            RuleFor(c => c.Request.ProductId).NotEmpty();
            RuleFor(c => c.Request.Rating).InclusiveBetween(1, 5);
            RuleFor(c => c.Request.Comment).MaximumLength(2000);
        }
    }
}
