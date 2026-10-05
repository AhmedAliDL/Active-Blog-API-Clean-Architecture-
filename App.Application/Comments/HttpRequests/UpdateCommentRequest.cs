namespace App.Application.Comments.HttpRequests
{
    public record UpdateCommentRequest(string CommentContent, Guid BlogId);
}
