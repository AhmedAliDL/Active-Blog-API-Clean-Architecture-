using FluentValidation;

namespace App.Application.Reports.Queries.GetReportById
{
    public class GetReportByIdValidator : AbstractValidator<GetReportByIdQuery>
    {
        public GetReportByIdValidator()
        {
            RuleFor(r => r.ReportId)
                .NotEmpty()
                .NotNull()
                .WithMessage("Report id is required.");
        }
    }
}
