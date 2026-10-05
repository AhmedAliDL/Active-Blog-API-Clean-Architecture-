using FluentValidation;

namespace App.Application.Follows.Commands.DeleteFollow
{
    public class DeleteFollowValidator : AbstractValidator<DeleteFollowCommand>
    {
        public DeleteFollowValidator()
        {
            RuleFor(b => b.BloggerId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blogger id is required.");
        }
    }
}
