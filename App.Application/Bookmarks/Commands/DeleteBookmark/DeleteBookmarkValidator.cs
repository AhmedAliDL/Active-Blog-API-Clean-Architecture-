using FluentValidation;

namespace App.Application.Bookmarks.Commands.DeleteBookmark
{
    public class DeleteBookmarkValidator : AbstractValidator<DeleteBookmarkCommand>
    {
        public DeleteBookmarkValidator()
        {
            RuleFor(l => l.BlogId)
               .NotNull()
               .NotEmpty()
               .WithMessage("Blog id is required.");
        }
    }
}
