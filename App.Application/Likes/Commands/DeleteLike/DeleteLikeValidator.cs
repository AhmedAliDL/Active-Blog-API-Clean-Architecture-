using FluentValidation;

namespace App.Application.Likes.Commands.DeleteLike
{
    public class DeleteLikeValidator : AbstractValidator<DeleteLikeCommand>
    {
        public DeleteLikeValidator()
        {
            RuleFor(l => l.BlogId)
              .NotNull()
              .NotEmpty()
              .WithMessage("Blog id is required.");
        }
    }
}
