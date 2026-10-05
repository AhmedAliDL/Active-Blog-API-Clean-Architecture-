using FluentValidation;

namespace App.Application.Comments.Command.DeleteComment
{
    public class DeleteCommentValidator : AbstractValidator<DeleteCommentCommand>
    {
        public DeleteCommentValidator()
        {
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
