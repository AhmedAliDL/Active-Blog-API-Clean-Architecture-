using FluentValidation;


namespace App.Application.ContentBlocks.Commands.EditContentBlocks
{
    public class EditContentBlocksValidator : AbstractValidator<EditContentBlocksCommand>
    {
        public EditContentBlocksValidator()
        {
            RuleFor(i => i.Data.Count)
                .GreaterThan(1)
                .WithMessage("Required at least 1 image.");
            RuleForEach(i => i.Data)
                .ChildRules(d =>
                {
                    d.RuleFor(b => b.Content)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Content is required for each content block.");
                    d.RuleFor(b => b.BlockId)
                    .NotEmpty()
                    .NotNull()
                    .WithMessage("Block id is required for each content block.");

                });
            RuleFor(i => i.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required.");

        }

    }
}
