using App.Domain.Enums;

namespace App.Application.ContentBlocks.Dto
{
    public record ContentBlockDto
    {
        public Guid BlockId { get; set; }
        public ContentBlockType Type { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
