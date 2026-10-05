using App.Domain.Enums;

namespace App.Application.ContentBlocks.Dto
{
    public record BlockDataDto
    {
        public ContentBlockType Type { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
