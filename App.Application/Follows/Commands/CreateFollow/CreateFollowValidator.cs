using FluentValidation;

namespace App.Application.Follows.Commands.CreateFollow
{
    public class CreateFollowValidator : AbstractValidator<CreateFollowCommand>
    {
        public CreateFollowValidator()
        {

            RuleFor(b => b.BloggerId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blogger id is required.");
        }
    }
}
