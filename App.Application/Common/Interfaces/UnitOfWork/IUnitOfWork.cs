using App.Application.Common.Interfaces.Repos;
using App.Domain.Entities;

namespace App.Application.Common.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Blog> Blogs { get; }
        IBaseRepository<Comment> Comments { get; }
        IBaseRepository<Category> Categories { get; }
        IBaseRepository<Tag> Tags { get; }
        IBaseRepository<ContentBlock> ContentBlocks { get; }
        IBaseRepository<Like> Likes { get; }
        IBaseRepository<Follow> Follows { get; }
        IBaseRepository<Bookmark> Bookmarks { get; }
        IBaseRepository<Report> Reports { get; }
        IBaseRepository<Notification> Notifications { get; }
        IBaseRepository<AuditLog> AuditLogs { get; }
        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }

}
