using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class Tag : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public Category Category { get; set; } = null!;
    }
}
