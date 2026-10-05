using FluentValidation;

namespace App.Application.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdValidator : AbstractValidator<GetCategoryByIdQuery>
    {
        public GetCategoryByIdValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .NotNull()
                .WithMessage("Category id is required.");
        }
    }
}
