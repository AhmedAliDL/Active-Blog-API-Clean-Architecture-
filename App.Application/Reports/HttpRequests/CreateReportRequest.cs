using App.Domain.Enums;

namespace App.Application.Reports.HttpRequests
{
    public record CreateReportRequest
    {
        public ReportReason Reason { get; set; }
        public string? Description { get; set; }
    }

}
