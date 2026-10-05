using FluentValidation;

namespace App.Application.Comments.Command.CreateComment
{
    public class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
    {
        public CreateCommentValidator()
        {
            RuleFor(c => c.CommentContent)
                .MinimumLength(1)
                .WithMessage("Comment must be at least 1 character")
                .MaximumLength(300)
                .WithMessage("Comment must be at most 300 characters");
            RuleFor(c => c.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required.");

        }
    }
}
