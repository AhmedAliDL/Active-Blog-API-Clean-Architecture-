using FluentValidation;

namespace App.Application.ContentBlocks.Commands.DeleteContentBlocks
{
    public class DeleteContentBlocksValidator : AbstractValidator<DeleteContentBlocksCommand>
    {
        public DeleteContentBlocksValidator()
        {
            RuleFor(i => i.BLogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required.");
            RuleFor(i => i.ContentBlocksIds.Count)
                .GreaterThan(1)
                .WithMessage("minimum ids count is 1.");
        }
    }
}
