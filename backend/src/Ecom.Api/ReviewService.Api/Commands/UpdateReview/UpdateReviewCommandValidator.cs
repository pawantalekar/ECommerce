using FluentValidation;

namespace ReviewService.Api.Commands.UpdateReview
{
    public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Request.Rating).InclusiveBetween(1, 5);
            RuleFor(c => c.Request.Comment).MaximumLength(2000);
        }
    }
}
