using FluentValidation;

namespace AuthService.Api.Commands
{
    public class RejectReviewCommandValidator : AbstractValidator<RejectReviewCommand>
    {
        public RejectReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();
            RuleFor(x => x.AdminId).NotEmpty();
        }
    }
}
