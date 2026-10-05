namespace App.Application.Comments.Dto
{
    public record CommentDto
    {
        public string UserName { get; set; } = null!;
        public string CommentContent { get; set; } = null!;
        public string? UserImage { get; set; }
        public DateTime CommentDate { get; set; }
        public List<CommentDto> Replies { get; set; } = [];
    }
}
