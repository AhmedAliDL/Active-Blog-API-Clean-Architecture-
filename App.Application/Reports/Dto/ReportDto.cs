using App.Domain.Enums;

namespace App.Application.Reports.Dto
{
    public record ReportDto
    {
        public Guid Id { get; set; }
        public ReportReason Reason { get; set; }
        public string? Description { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public Guid ReporterId { get; set; }
        public Guid? ReviewedById { get; set; }
        public Guid BlogId { get; set; }
    }
}
