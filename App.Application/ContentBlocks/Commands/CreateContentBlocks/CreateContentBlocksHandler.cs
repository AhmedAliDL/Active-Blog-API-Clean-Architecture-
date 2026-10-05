using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.ContentBlocks.Commands.CreateContentBlocks
{
    public class CreateContentBlocksHandler(IUnitOfWork unitOfWork, ILogger<CreateContentBlocksHandler> logger) : IRequestHandler<CreateContentBlocksCommand, int>
    {
        public async Task<int> Handle(CreateContentBlocksCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create content blocks operation started.");
            int order = 1;
            List<ContentBlock> contentBlocks = [.. request.Data.Select(i =>
            new ContentBlock
            {
                Type = i.Type,
                Data = i.Content,
                Order = order++,
                BlogId = request.BlogId
            })];

            await unitOfWork.ContentBlocks.AddRangeAsync(contentBlocks, cancellationToken);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Create content blocks operation completed.");
            return result;
        }
    }
}
