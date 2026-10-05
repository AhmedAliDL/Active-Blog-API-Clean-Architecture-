using FluentValidation;

namespace App.Application.Comments.Queries.GetCommentById
{
    public class GetCommentByIdValidator : AbstractValidator<GetCommentByIdQuery>
    {
        public GetCommentByIdValidator()
        {
            RuleFor(c => c.CommentId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Comment id is required.");
        }
    }
}
