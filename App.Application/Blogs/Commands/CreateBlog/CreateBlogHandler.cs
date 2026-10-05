using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Blogs.Commands.CreateBlog
{
    public class CreateBlogHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<CreateBlogHandler> logger) : IRequestHandler<CreateBlogCommand, int>
    {
        public async Task<int> Handle(CreateBlogCommand request, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Blog creation operation started.");
            Blog blog = new()
            {
                Title = request.Title,
                Image = request.ImagePath,
                CategoryId = request.CategoryId
            };
            Guid? userId = currentUserService.UserId;
            if (userId is not null && userId != Guid.Empty)
            {
                blog.UserId = userId!.Value;
                await unitOfWork.Blogs.AddAsync(blog, cancellationToken);
                var res = await unitOfWork.CompleteAsync(cancellationToken);
                logger.LogInformation("Blog creation operation completed.");
                return res;
            }
            else
                throw new NotFoundException("User not found.");
        }
    }
}
