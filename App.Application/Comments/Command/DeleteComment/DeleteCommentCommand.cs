using MediatR;

namespace App.Application.Comments.Command.DeleteComment
{
    public record DeleteCommentCommand(Guid CommentId, Guid BlogId) : IRequest<int>;
}
