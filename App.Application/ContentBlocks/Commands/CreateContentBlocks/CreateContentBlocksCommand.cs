using App.Application.ContentBlocks.Dto;
using MediatR;

namespace App.Application.ContentBlocks.Commands.CreateContentBlocks
{
    public record CreateContentBlocksCommand(Guid BlogId, List<BlockDataDto> Data) : IRequest<int>;
}
