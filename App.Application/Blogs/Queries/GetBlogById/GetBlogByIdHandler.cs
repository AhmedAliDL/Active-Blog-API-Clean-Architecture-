using App.Application.Blogs.Dto;
using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Blogs.Queries.GetBlogById
{
    public class GetBlogByIdHandler(IUnitOfWork unitOfWork, ILogger<GetBlogByIdHandler> logger) : IRequestHandler<GetBlogByIdQuery, BlogDetailsDto?>
    {
        public async Task<BlogDetailsDto?> Handle(GetBlogByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get blog by ID operation started.");
            Blog? blog = await unitOfWork.Blogs
               .FindAsync(b => request.BlogId == b.Id, [b => b.User], cancellationToken) ?? throw new NotFoundException("Blog not found.");

            var blogDetails = new BlogDetailsDto();
            if (blog != null)
            {
                blogDetails.BlogId = blog.Id;
                blogDetails.Title = blog.Title;
                blogDetails.CreatedDate = blog.CreatedAt;
                blogDetails.CategoryId = blog.CategoryId;
                blogDetails.BlogImage = blog.Image!;
                blogDetails.UserImage = blog.User.Image!;
                blogDetails.UserName = $"{blog.User.FName} {blog.User.LName}";
            }
            logger.LogInformation("Get blog by ID operation completed.");
            return blogDetails;
        }
    }
}
