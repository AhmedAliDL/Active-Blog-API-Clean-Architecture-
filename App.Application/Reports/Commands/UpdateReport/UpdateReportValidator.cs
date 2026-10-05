using FluentValidation;

namespace App.Application.Reports.Commands.UpdateReport
{
    public class UpdateReportValidator : AbstractValidator<UpdateReportCommand>
    {
        public UpdateReportValidator()
        {
            RuleFor(r => r.ReportId)
                .NotNull()
                .NotEmpty()
                .WithMessage("Report id is required.");
            RuleFor(r => r.Status)
                .IsInEnum()
                .WithMessage("Invalid report status.");
        }
    }
}
