using App.Domain.Enums;
using FluentValidation;
namespace App.Application.ContentBlocks.Commands.CreateContentBlocks
{
    public class CreateContentBlocksValidator : AbstractValidator<CreateContentBlocksCommand>
    {
        public CreateContentBlocksValidator()
        {
            RuleForEach(i => i.Data)
             .ChildRules(d =>
             {
                 d.RuleFor(b => b.Type)
             .IsInEnum()
             .WithMessage("Invalid content block type.");

                 d.RuleFor(b => b.Content)
                     .NotEmpty()
                     .WithMessage("Content is required for text content blocks.")
                     .When(b =>
                         b.Type == ContentBlockType.Text ||
                         b.Type == ContentBlockType.Quote ||
                         b.Type == ContentBlockType.Heading ||
                         b.Type == ContentBlockType.Code);

                 d.RuleFor(b => b.Content)
                     .Must(BeValidUrl)
                     .WithMessage("Content must be a valid URL for video and image content blocks.")
                     .When(b =>
                         b.Type == ContentBlockType.Video ||
                         b.Type == ContentBlockType.Image);
             });

        }
        private static bool BeValidUrl(string? url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                   && (uri.Scheme == Uri.UriSchemeHttp ||
                       uri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
