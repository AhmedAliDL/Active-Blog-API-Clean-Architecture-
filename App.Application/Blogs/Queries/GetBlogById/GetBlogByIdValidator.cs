using FluentValidation;

namespace App.Application.Blogs.Queries.GetBlogById
{
    public class GetBlogByIdValidator : AbstractValidator<GetBlogByIdQuery>
    {
        public GetBlogByIdValidator()
        {
            RuleFor(b => b.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required");
        }
    }
}
