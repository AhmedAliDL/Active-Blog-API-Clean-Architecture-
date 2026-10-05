using FluentValidation;

namespace App.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator()
        {
            RuleFor(c => c.CategoryName)
                .MinimumLength(2)
                .WithMessage("CategoryName with minimum 2 character.")
                .MaximumLength(20)
                .WithMessage("CategoryName with maximum 20 character.");

        }
    }
}
