using MediatR;

namespace App.Application.ContentBlocks.Commands.DeleteContentBlocks
{
    public record DeleteContentBlocksCommand(Guid BLogId, HashSet<Guid> ContentBlocksIds) : IRequest<int>;
}
