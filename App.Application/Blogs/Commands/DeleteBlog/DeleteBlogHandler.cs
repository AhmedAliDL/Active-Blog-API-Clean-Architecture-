using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Blogs.Commands.DeleteBlog
{
    public class DeleteBlogHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<DeleteBlogHandler> logger) : IRequestHandler<DeleteBlogCommand, int>
    {
        public async Task<int> Handle(DeleteBlogCommand request, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Delete blog operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty) throw new NotFoundException("User Not found");
            Blog? blog = await unitOfWork.Blogs.GetByIdAsync(request.BlogId) ?? throw new NotFoundException("Blog Not Found.");
            if (blog.UserId != userId) throw new ForbiddenException("You don`t have permission to delete this blog.");

            unitOfWork.Blogs.Delete(blog);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Delete blog operation completed.");
            return result;
        }
    }
}
