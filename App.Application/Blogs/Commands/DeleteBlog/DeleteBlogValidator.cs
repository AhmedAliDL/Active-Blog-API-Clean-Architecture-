using FluentValidation;

namespace App.Application.Blogs.Commands.DeleteBlog
{
    public class DeleteBlogValidator : AbstractValidator<DeleteBlogCommand>
    {
        public DeleteBlogValidator()
        {
            RuleFor(b => b.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required.");
        }
    }
}
