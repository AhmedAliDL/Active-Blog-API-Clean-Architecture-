using MediatR;

namespace App.Application.Blogs.HttpRequests
{
    public record UpdateBlogRequest : IRequest<int>
    {
        public string? Title { get; set; }
        public Guid? CategoryId { get; set; }
        public string? ImagePath { get; set; }
    }
}
