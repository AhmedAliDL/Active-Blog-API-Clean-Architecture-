using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class Comment : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public string CommentContent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ParentCommentId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public Blog Blog { get; set; } = null!;
        public Comment? ParentComment { get; set; } = null;
        public User User { get; set; } = null!;

        public ICollection<Comment> Replies { get; set; } = null!;

    }
}
