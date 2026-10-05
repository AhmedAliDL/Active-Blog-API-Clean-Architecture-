using App.Application.Blogs.Dto;
using MediatR;

namespace App.Application.Blogs.Queries.GetAllBlogs
{
    public record GetAllBlogsQuery : IRequest<List<BlogDetailsDto>>;
}
