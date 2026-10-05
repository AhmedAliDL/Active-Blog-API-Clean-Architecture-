using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.ContentBlocks.Commands.DeleteContentBlocks
{
    public class DeleteContentBlocksHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<DeleteContentBlocksHandler> logger) : IRequestHandler<DeleteContentBlocksCommand, int>
    {
        public async Task<int> Handle(DeleteContentBlocksCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete content blocks operation started.");
            if (currentUserService.UserId == Guid.Empty) throw new NotFoundException("User not found.");
            List<ContentBlock> contentBlocks = (await unitOfWork.ContentBlocks.FindAllAsync(bi => bi.BlogId == request.BLogId && request.ContentBlocksIds.Contains(bi.Id), cancellationToken)).ToList() ?? throw new NotFoundException("Blog Not Found.");

            if (contentBlocks != null && contentBlocks.Count != 0)
            {
                unitOfWork.ContentBlocks.DeleteRange(contentBlocks);
                var result = await unitOfWork.CompleteAsync(cancellationToken);
                logger.LogInformation("Delete content blocks operation completed.");
                return result;
            }
            else
                throw new ForbiddenException("This User not Allowed to Rmeove these images");
        }
    }
}
