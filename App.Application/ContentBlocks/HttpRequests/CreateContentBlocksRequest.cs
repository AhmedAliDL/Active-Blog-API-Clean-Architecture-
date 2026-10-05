using App.Application.ContentBlocks.Dto;

namespace App.Application.ContentBlocks.HttpRequests
{
    public record CreateContentBlocksRequest(List<BlockDataDto> Data);
}
