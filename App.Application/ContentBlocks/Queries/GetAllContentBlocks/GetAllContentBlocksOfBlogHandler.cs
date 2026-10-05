using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.ContentBlocks.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.ContentBlocks.Queries.GetAllContentBlocks
{
    public class GetAllContentBlocksOfBlogHandler(IUnitOfWork unitOfWork, ILogger<GetAllContentBlocksOfBlogHandler> logger) : IRequestHandler<GetAllContentBlocksOfBlogQuery, List<ContentBlockDto>>
    {
        public async Task<List<ContentBlockDto>> Handle(GetAllContentBlocksOfBlogQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all content blocks of blog operation started.");
            List<ContentBlockDto> result = [.. (await unitOfWork.ContentBlocks.FindAllAsync(i => i.BlogId == request.BlogId, cancellationToken))
                .Select(i => new ContentBlockDto{
                    BlockId = i.Id,
                    Type = i.Type,
                    Content = i.Data,
                })];
            logger.LogInformation("Get all content blocks of blog operation completed.");
            return result;
        }
    }
}
