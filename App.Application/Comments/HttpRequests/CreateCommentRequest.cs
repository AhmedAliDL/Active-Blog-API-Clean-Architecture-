namespace App.Application.Comments.HttpRequests
{
    public record CreateCommentRequest(string CommentContent ,Guid? ParentCommentId);
}
