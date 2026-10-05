using FluentValidation;

namespace App.Application.Likes.Commands.CreateLike
{
    public class CreateLikeValidator : AbstractValidator<CreateLikeCommand>
    {
        public CreateLikeValidator()
        {
            RuleFor(l => l.BlogId)
               .NotNull()
               .NotEmpty()
               .WithMessage("Blog id is required.");
        }
    }
}
