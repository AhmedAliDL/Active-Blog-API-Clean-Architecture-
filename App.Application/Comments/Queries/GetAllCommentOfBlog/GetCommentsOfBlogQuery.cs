using App.Application.Comments.Dto;
using MediatR;

namespace App.Application.Comments.Queries.GetAllCommentOfBlog
{
    public record GetCommentsOfBlogQuery(Guid BlogId) : IRequest<List<CommentDto>>;
}
