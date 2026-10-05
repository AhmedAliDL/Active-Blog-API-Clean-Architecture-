using FluentValidation;

namespace App.Application.Reports.Commands.CreateReport
{
    public class CreateReportValidator : AbstractValidator<CreateReportCommand>
    {
        public CreateReportValidator()
        {
            RuleFor(r => r.Reason)
                .IsInEnum()
                .WithMessage("Reason must be one of the following values: Spam, InappropriateContent, Harassment, Other.");
            RuleFor(r => r.BlogId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Blog id is required.");

            RuleFor(r => r.Description)
                .MinimumLength(3)
                .MaximumLength(300)
                .WithMessage("Description  Must be between 3 and 300 character");


        }
    }
}
