using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler(IUnitOfWork unitOfWork, ILogger<CreateCategoryHandler> logger) : IRequestHandler<CreateCategoryCommand, int>
    {
        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create category operation started.");
            Category category = new()
            {
                CategoryName = request.CategoryName
            };

            await unitOfWork.Categories.AddAsync(category, cancellationToken);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Create category operation completed.");
            return result;
        }
    }
}