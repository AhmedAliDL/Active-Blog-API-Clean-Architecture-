using FluentValidation;

namespace App.Application.Blogs.Commands.UpdateBlog
{
    public class UpdateBlogValidator : AbstractValidator<UpdateBlogCommand>
    {
        public UpdateBlogValidator()
        {
            RuleFor(b => b.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required.");
            RuleFor(b => b.Title)
           .MinimumLength(2)
           .WithMessage("Title with minimum 2 character.")
           .MaximumLength(20)
           .WithMessage("Title with maximum 20 character.");


        }
    }
}
