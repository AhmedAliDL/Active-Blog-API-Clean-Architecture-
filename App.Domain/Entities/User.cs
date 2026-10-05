using App.Domain.Interceptors;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Entities
{
    public class User : IdentityUser<Guid>, IAuditLog
    {
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime LastLoginAt { get; set; }
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = null!;
        public ICollection<Blog> UserBlogs { get; set; } = null!;
        public ICollection<Comment> UserComments { get; set; } = null!;
        public ICollection<Like> UserLikes { get; set; } = null!;
        public ICollection<Follow> Followers { get; set; } = null!;
        public ICollection<Follow> Blogger { get; set; } = null!;
        public ICollection<Bookmark> Bookmarks { get; set; } = null!;
        public ICollection<Report> ReporterReports { get; set; } = null!;
        public ICollection<Report> ReviewerReports { get; set; } = null!;
        public ICollection<Notification> SenderNotifications { get; set; } = null!;
        public ICollection<Notification> RecieverNotifications { get; set; } = null!;
        public ICollection<AuditLog> AuditLogs { get; set; } = null!;
    }


}
