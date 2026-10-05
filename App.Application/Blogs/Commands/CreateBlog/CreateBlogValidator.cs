using FluentValidation;

namespace App.Application.Blogs.Commands.CreateBlog
{
    public class CreateBlogValidator : AbstractValidator<CreateBlogCommand>
    {
        public CreateBlogValidator()
        {
            RuleFor(b => b.Title)
                .MinimumLength(2)
                .WithMessage("Title with minimum 2 character.")
                .MaximumLength(20)
                .WithMessage("Title with maximum 20 character.")
                .NotEmpty()
                .NotNull()
                .WithMessage("Title is is required.");

            RuleFor(b => b.CategoryId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Category id is required.");

            RuleFor(b => b.ImagePath)
                .NotEmpty()
                .NotNull()
                .WithMessage("Image path is required.");
        }
    }
}
