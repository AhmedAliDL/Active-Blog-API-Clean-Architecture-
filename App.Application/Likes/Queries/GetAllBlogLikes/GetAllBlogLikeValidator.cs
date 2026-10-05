using FluentValidation;

namespace App.Application.Likes.Queries.GetAllBlogLikes
{
    public class GetAllBlogLikeValidator : AbstractValidator<GetAllBlogLikesQuery>
    {
        public GetAllBlogLikeValidator()
        {
            RuleFor(l =>  l.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required.");
        }
    }
}
