using App.Domain.Enums;
using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class Report : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public ReportReason Reason { get; set; }
        public string? Description { get; set; }
        public ReportStatus Status { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime? ReviewedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public Guid ReporterId { get; set; }
        public Guid? ReviewerId { get; set; }
        public Guid BlogId { get; set; }

        public User Reporter { get; set; } = null!;
        public User Reviewer { get; set; } = null!;
        public Blog Blog { get; set; } = null!;
    }

}
