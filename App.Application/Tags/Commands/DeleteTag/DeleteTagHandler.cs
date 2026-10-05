using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Tags.Commands.DeleteTag
{
    public class DeleteTagHandler(IUnitOfWork unitOfWork, ILogger<DeleteTagHandler> logger) : IRequestHandler<DeleteTagCommand, int>
    {
        public async Task<int> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete tag operation started.");
            Tag? tag = await unitOfWork.Tags.GetByIdAsync(request.TagId) ?? throw new NotFoundException("Tag Not Found.");

            unitOfWork.Tags.Delete(tag);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Delete tag operation completed.");
            return result;
        }
    }
}
