using MediatR;

namespace App.Application.Comments.Command.UpdateComment
{
    public record UpdateCommentCommand(Guid CommentId, string CommentContent, Guid BlogId) : IRequest<int>;
}
