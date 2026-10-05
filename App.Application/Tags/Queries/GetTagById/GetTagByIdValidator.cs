using FluentValidation;

namespace App.Application.Tags.Queries.GetTagById
{
    public class GetTagByIdValidator : AbstractValidator<GetTagByIdQuery>
    {
        public GetTagByIdValidator()
        {
            RuleFor(t => t.TagId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Tag id is required.");
        }
    }
}
