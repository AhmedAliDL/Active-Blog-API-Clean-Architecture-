namespace App.Application.Comments.Dto
{
    public record GetCommentDto
    {
        public string CommentContent { get; set; } = null!;
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}
