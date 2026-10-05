using App.Domain.Enums;
using MediatR;

namespace App.Application.Reports.Commands.UpdateReport
{
    public record UpdateReportCommand(Guid ReportId, ReportStatus Status) : IRequest<int>;
}
