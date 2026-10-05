using FluentValidation;

namespace App.Application.Tags.Commands.DeleteTag
{
    public class DeleteTagValidator : AbstractValidator<DeleteTagCommand>
    {
        public DeleteTagValidator()
        {
            RuleFor(c => c.TagId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Tag id is required.");
        }
    }
}
