using App.Application.Blogs.Dto;
using App.Application.Common.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Blogs.Queries.GetAllBlogs
{
    public class GetAllBlogsHandler(IUnitOfWork unitOfWork, ILogger<GetAllBlogsHandler> logger) : IRequestHandler<GetAllBlogsQuery, List<BlogDetailsDto>>
    {
        public async Task<List<BlogDetailsDto>> Handle(GetAllBlogsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all blogs operation started.");
            var blogsIncludingUser = (await unitOfWork.Blogs.FindAllAsync(
                criteria: b => b.Id != Guid.Empty,
                includes: [b => b.User],
                cancellationToken: cancellationToken)).Select(b => new BlogDetailsDto
                {
                    BlogId = b.Id,
                    Title = b.Title,
                    CategoryId = b.CategoryId,
                    BlogImage = b.Image!,
                    CreatedDate = b.CreatedAt,
                    UserName = $"{b.User.FName} {b.User.LName}",
                    UserImage = b.User.Image!
                }).ToList();
            logger.LogInformation("Get all blogs operation completed.");
            return blogsIncludingUser;
        }
    }
}
