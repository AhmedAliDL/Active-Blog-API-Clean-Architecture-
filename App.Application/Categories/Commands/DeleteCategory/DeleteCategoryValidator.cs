using FluentValidation;

namespace App.Application.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
    {
        public DeleteCategoryValidator()
        {
            RuleFor(c => c.CategoryId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Category id is required.");
        }
    }
}
