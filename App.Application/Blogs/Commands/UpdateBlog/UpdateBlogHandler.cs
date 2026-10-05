using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Blogs.Commands.UpdateBlog
{
    public class UpdateBlogHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<UpdateBlogHandler> logger) : IRequestHandler<UpdateBlogCommand, int>
    {
        public async Task<int> Handle(UpdateBlogCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Update blog operation started.");
            Guid? userId = currentUserService.UserId;

            if (userId is null || userId == Guid.Empty) throw new NotFoundException("User Not Found.");
            Blog? oldBlog = await unitOfWork.Blogs.GetByIdAsync(request.BlogId) ?? throw new NotFoundException("Blog Not Found.");

            if (oldBlog.UserId != userId) throw new ForbiddenException("You Don`t have permission to edit this blog.");

            oldBlog.Title = request.Title ?? oldBlog.Title;
            oldBlog.Image = request.ImagePath ?? oldBlog.Image;
            oldBlog.UpdatedAt = DateTime.UtcNow;
            oldBlog.CategoryId = request.CategoryId ?? oldBlog.CategoryId;

            unitOfWork.Blogs.Update(oldBlog);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Update blog operation completed.");
            return result;
        }
    }
}
