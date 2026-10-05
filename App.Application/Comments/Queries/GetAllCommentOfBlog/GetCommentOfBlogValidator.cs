using FluentValidation;

namespace App.Application.Comments.Queries.GetAllCommentOfBlog
{
    public class GetCommentOfBlogValidator : AbstractValidator<GetCommentsOfBlogQuery>
    {
        public GetCommentOfBlogValidator()
        {
            RuleFor(c => c.BlogId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Blog id is required.");
        }
    }
}
