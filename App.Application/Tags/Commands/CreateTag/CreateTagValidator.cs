using FluentValidation;

namespace App.Application.Tags.Commands.CreateTag
{
    public class CreateTagValidator : AbstractValidator<CreateTagCommand>
    {
        public CreateTagValidator()
        {
            RuleFor(c => c.TagName)
                .MinimumLength(2)
                .WithMessage("TagName with minimum 2 character.")
                .MaximumLength(12)
                .WithMessage("TagName with maximum 12 character.");

        }
    }
}
