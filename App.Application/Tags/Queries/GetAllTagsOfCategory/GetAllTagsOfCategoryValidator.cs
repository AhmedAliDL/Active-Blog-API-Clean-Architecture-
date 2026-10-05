using FluentValidation;

namespace App.Application.Tags.Queries.GetAllTagsOfCategory
{
    public class GetAllTagsOfCategoryValidator : AbstractValidator<GetAllTagsOfCategoryQuery>
    {
        public GetAllTagsOfCategoryValidator()
        {
            RuleFor(t => t.CatId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Category id is required.");
        }
    }
}
