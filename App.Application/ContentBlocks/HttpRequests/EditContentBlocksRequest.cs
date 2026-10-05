using App.Application.ContentBlocks.Dto;

namespace App.Application.ContentBlocks.HttpRequests
{
    public record EditContentBlocksRequest(List<EditContentBlockDto> Data);
}
