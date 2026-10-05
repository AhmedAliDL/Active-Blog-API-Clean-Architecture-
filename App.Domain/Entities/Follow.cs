using App.Domain.Interceptors;

namespace App.Domain.Entities
{
    public class Follow : ISoftDeletable, IAuditLog
    {
        public Guid Id { get; set; }
        public Guid BloggerId { get; set; }
        public Guid FollowerId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }

        public User Blogger { get; set; } = null!;
        public User Follower { get; set; } = null!;
    }

}
