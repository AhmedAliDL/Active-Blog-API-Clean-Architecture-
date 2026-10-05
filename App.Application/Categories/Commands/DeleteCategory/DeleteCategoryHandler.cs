using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryHandler(IUnitOfWork unitOfWork, ILogger<DeleteCategoryHandler> logger) : IRequestHandler<DeleteCategoryCommand, int>
    {
        public async Task<int> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete category operation started.");
            Category? category = await unitOfWork.Categories.GetByIdAsync(request.CategoryId) ?? throw new NotFoundException("Category Not Found.");

            unitOfWork.Categories.Delete(category);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Delete category operation completed.");
            return result;
        }
    }
}
