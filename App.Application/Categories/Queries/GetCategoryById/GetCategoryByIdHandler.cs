using App.Application.Categories.Dto;
using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdHandler(IUnitOfWork unitOfWork, ILogger<GetCategoryByIdHandler> logger) : IRequestHandler<GetCategoryByIdQuery, CategoryDetailsDto?>
    {
        public async Task<CategoryDetailsDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get category by ID operation started.");
            Category? category = await unitOfWork.Categories
               .GetByIdAsync(request.Id) ?? throw new NotFoundException("Category is not found.");
            var catDetails = new CategoryDetailsDto
            {
                CategoryId = category.Id,
                CategoryName = category.CategoryName
            };
            logger.LogInformation("Get category by ID operation completed.");
            return catDetails;
        }
    }
}
