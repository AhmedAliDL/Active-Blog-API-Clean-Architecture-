using FluentValidation;

namespace App.Application.ContentBlocks.Queries.GetAllContentBlocks
{
    public class GetAllBlogImagesValidator : AbstractValidator<GetAllContentBlocksOfBlogQuery>
    {
        public GetAllBlogImagesValidator()
        {
            RuleFor(bc => bc.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("blog id is required.");
        }
    }
}
