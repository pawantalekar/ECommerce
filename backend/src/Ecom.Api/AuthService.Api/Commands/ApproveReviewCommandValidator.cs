using FluentValidation;

namespace AuthService.Api.Commands
{
    public class ApproveReviewCommandValidator : AbstractValidator<ApproveReviewCommand>
    {
        public ApproveReviewCommandValidator()
        {
            RuleFor(x => x.ReviewId).NotEmpty();
            RuleFor(x => x.AdminId).NotEmpty();
        }
    }
}
