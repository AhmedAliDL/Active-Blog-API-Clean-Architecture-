using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class Like : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public Blog Blog { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
