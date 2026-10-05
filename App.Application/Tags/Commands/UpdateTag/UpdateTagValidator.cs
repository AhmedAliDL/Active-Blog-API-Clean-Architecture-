using FluentValidation;

namespace App.Application.Tags.Commands.UpdateTag
{
    public class UpdateTagValidator : AbstractValidator<UpdateTagCommand>
    {
        public UpdateTagValidator()
        {
            RuleFor(b => b.CategoryId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Category id is required.");
            RuleFor(b => b.TagId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Tag id is required.");
            RuleFor(b => b.TagName)
           .MinimumLength(2)
           .WithMessage("Tag name with minimum 2 character.")
           .MaximumLength(12)
           .WithMessage("Tag name with maximum 12 character.");

        }
    }
}
