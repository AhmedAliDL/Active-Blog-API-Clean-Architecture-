using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class Category : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public ICollection<Blog> Blogs { get; set; } = null!;
        public ICollection<Tag> Tags { get; set; } = null!;
    }

}
