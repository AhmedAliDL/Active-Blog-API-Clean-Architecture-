namespace App.Application.Blogs.Dto
{
    public class BlogDetailsDto
    {
        public Guid BlogId { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string BlogImage { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserImage { get; set; } = string.Empty;

    }
}
