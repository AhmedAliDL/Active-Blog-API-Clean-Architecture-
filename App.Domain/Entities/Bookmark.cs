using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class Bookmark : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public Blog Blog { get; set; } = null!;
        public User User { get; set; } = null!;
    }

}
