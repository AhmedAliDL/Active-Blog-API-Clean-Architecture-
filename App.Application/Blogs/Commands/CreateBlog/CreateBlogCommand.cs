using App.Application.Common.ValidationAttributes;
using MediatR;

namespace App.Application.Blogs.Commands.CreateBlog
{
    public record CreateBlogCommand : IRequest<int>
    {
        public string Title { get; init; } = null!;

        public Guid CategoryId { get; init; }
        [CheckImageExtension(errorMessage: "Invalid image file format. Only .jpg, .png, and .jpeg files are allowed.")]
        public string? ImagePath { get; init; }
    }
}
