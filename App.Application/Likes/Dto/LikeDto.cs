namespace App.Application.Likes.Dto
{
    public record LikeDto
    {
        public Guid LikeId { get; set; }
        public Guid BlogId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
