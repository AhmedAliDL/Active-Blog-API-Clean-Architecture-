using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Tags.Commands.UpdateTag
{
    public class UpdateTagHandler(IUnitOfWork unitOfWork, ILogger<UpdateTagHandler> logger) : IRequestHandler<UpdateTagCommand, int>
    {
        public async Task<int> Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Update tag operation started.");
            Tag? oldTag = await unitOfWork.Tags.GetByIdAsync(request.TagId) ?? throw new NotFoundException("Tag Not Found.");

            oldTag.Name = request.TagName ?? oldTag.Name;
            oldTag.CategoryId = request.CategoryId == Guid.Empty ? oldTag.CategoryId : request.CategoryId;
            oldTag.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Tags.Update(oldTag);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Update tag operation completed.");
            return result;
        }
    }
}
