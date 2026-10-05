using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryHandler(IUnitOfWork unitOfWork, ILogger<UpdateCategoryHandler> logger) : IRequestHandler<UpdateCategoryCommand, int>
    {
        public async Task<int> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Update category operation started.");
            Category? oldCategory = await unitOfWork.Categories.GetByIdAsync(request.CategoryId) ?? throw new NotFoundException("Category Not Found.");

            oldCategory.CategoryName = request.CategoryName ?? oldCategory.CategoryName;
            oldCategory.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Categories.Update(oldCategory);
            var res = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Update category operation started.");
            return res;
        }
    }
}
