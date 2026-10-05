using MediatR;

namespace App.Application.Blogs.Commands.DeleteBlog
{
    public record DeleteBlogCommand(Guid BlogId) : IRequest<int>;
}
