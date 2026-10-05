using App.Application.Common.ValidationAttributes;
using MediatR;

namespace App.Application.Blogs.Commands.UpdateBlog
{
    public record UpdateBlogCommand : IRequest<int>
    {
        public Guid BlogId { get; set; }
        public string? Title { get; set; }
        public Guid? CategoryId { get; set; }
        [CheckImageExtension(errorMessage: "Invalid image file format. Only .jpg, .png, and .jpeg files are allowed.")]
        public string? ImagePath { get; set; }
    }
}
