using FluentValidation;

namespace App.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(b => b.CategoryId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Category id is required.");
            RuleFor(b => b.CategoryName)
           .MinimumLength(2)
           .WithMessage("Category name with minimum 2 character.")
           .MaximumLength(20)
           .WithMessage("Category name with maximum 20 character.");

        }
    }
}
