using App.Application.Common.Interfaces.Repos;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using App.Infrastructure.Persistance;
using App.Infrastructure.Repositories;

namespace App.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Blogs = new BaseRepository<Blog>(_context);
            Comments = new BaseRepository<Comment>(_context);
            Categories = new BaseRepository<Category>(_context);
            Tags = new BaseRepository<Tag>(_context);
            ContentBlocks = new BaseRepository<ContentBlock>(_context);
            Likes = new BaseRepository<Like>(_context);
            Follows = new BaseRepository<Follow>(_context);
            Bookmarks = new BaseRepository<Bookmark>(_context);
            Reports = new BaseRepository<Report>(_context);
            Notifications = new BaseRepository<Notification>(_context);
            AuditLogs = new BaseRepository<AuditLog>(_context);
        }
        public IBaseRepository<Blog> Blogs { get; private set; }
        public IBaseRepository<Comment> Comments { get; private set; }
        public IBaseRepository<Category> Categories { get; private set; }
        public IBaseRepository<Tag> Tags { get; private set; }
        public IBaseRepository<ContentBlock> ContentBlocks { get; private set; }
        public IBaseRepository<Like> Likes { get; private set; }
        public IBaseRepository<Follow> Follows { get; private set; }
        public IBaseRepository<Bookmark> Bookmarks { get; private set; }
        public IBaseRepository<Report> Reports { get; private set; }
        public IBaseRepository<Notification> Notifications { get; private set; }
        public IBaseRepository<AuditLog> AuditLogs { get; private set; }


        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _context.Dispose();
        }
    }
}
