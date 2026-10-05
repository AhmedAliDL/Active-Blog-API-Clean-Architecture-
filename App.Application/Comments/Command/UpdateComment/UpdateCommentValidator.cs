using FluentValidation;

namespace App.Application.Comments.Command.UpdateComment
{
    public class UpdateCommentValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentValidator()
        {
            RuleFor(c => c.CommentContent)
                .MinimumLength(1)
                .WithMessage("Comment must be 1 character at least.")
                .MaximumLength(300)
                .WithMessage("Comment must be at most 300 characters");
            RuleFor(c => c.CommentId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Comment id is required.");
            RuleFor(c => c.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required.");
        }
    }
}
