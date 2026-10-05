using App.Application.ContentBlocks.Dto;
using MediatR;


namespace App.Application.ContentBlocks.Commands.EditContentBlocks
{
    public record EditContentBlocksCommand(Guid BlogId, List<EditContentBlockDto> Data) : IRequest<int>;
}
