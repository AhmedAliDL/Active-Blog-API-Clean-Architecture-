using App.Domain.Enums;
using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class ContentBlock : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public ContentBlockType Type { get; set; }
        public string Data { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid BlogId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public Blog Blog { get; set; } = null!;
    }
}
