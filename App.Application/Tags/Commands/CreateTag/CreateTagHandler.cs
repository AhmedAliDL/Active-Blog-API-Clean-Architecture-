using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Tags.Commands.CreateTag
{
    public class CreateTagHandler(IUnitOfWork unitOfWork, ILogger<CreateTagHandler> logger) : IRequestHandler<CreateTagCommand, int>
    {
        public async Task<int> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create tag operation started.");
            _ = await unitOfWork.Categories.GetByIdAsync(request.CategoryId) ?? throw new NotFoundException("Category is not found.");
            Tag tag = new()
            {
                Name = request.TagName,
                CategoryId = request.CategoryId
            };

            await unitOfWork.Tags.AddAsync(tag, cancellationToken);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Create tag operation completed.");
            return result;
        }
    }
}