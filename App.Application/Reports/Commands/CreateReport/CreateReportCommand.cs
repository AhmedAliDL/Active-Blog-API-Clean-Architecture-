using App.Domain.Enums;
using MediatR;

namespace App.Application.Reports.Commands.CreateReport
{
    public record CreateReportCommand : IRequest<int>
    {
        public ReportReason Reason { get; set; }
        public string? Description { get; set; }
        public Guid BlogId { get; set; }
    }
}
