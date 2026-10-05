using MediatR;

namespace App.Application.Comments.Command.CreateComment
{
    public record CreateCommentCommand : IRequest<int>
    {
        public string CommentContent { get; init; } = null!;
        public Guid BlogId { get; init; }
        public Guid? ParentCommentId { get; init; }
    }

}
