using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class Blog : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Image { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public Category Category { get; set; } = null!;
        public User User { get; set; } = null!;

        public ICollection<Comment> BlogComments { get; set; } = null!;
        public ICollection<ContentBlock> ContentBlocks { get; set; } = null!;
        public ICollection<Like> BlogLikes { get; set; } = null!;
        public ICollection<Bookmark> Bookmarks { get; set; } = null!;
        public ICollection<Report> Reports { get; set; } = null!;

    }
}
