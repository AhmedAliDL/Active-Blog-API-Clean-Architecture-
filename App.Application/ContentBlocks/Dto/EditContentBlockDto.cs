namespace App.Application.ContentBlocks.Dto
{
    public record EditContentBlockDto
    {
        public Guid BlockId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
