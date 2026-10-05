using App.Application.Blogs.Dto;
using MediatR;

namespace App.Application.Blogs.Queries.GetBlogById
{
    public record GetBlogByIdQuery(Guid BlogId) : IRequest<BlogDetailsDto?>;
}
