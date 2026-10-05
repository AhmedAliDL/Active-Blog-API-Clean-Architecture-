using FluentValidation;

namespace App.Application.Bookmarks.Commands.CreateBookmark
{
    public class CreateBookmarkValidator : AbstractValidator<CreateBookmarkCommand>
    {
        public CreateBookmarkValidator()
        {
            RuleFor(l => l.BlogId)
               .NotNull()
               .NotEmpty()
               .WithMessage("Blog id is required.");
        }
    }
}
