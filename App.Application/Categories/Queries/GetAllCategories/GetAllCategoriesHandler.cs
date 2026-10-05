using App.Application.Categories.Dto;
using App.Application.Common.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Categories.Queries.GetAllCategories
{
    public class GetAllCategoriesHandler(IUnitOfWork unitOfWork, ILogger<GetAllCategoriesHandler> logger) : IRequestHandler<GetAllCategoriesQuery, List<CategoryDetailsDto>>
    {
        public async Task<List<CategoryDetailsDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all categories operation started.");
            List<CategoryDetailsDto> result = [.. (await unitOfWork.Categories.GetAllAsync(cancellationToken)).Select(c => new CategoryDetailsDto
            {
                CategoryId = c.Id,
                CategoryName = c.CategoryName
            })];
            logger.LogInformation("Get all categories operation completed.");
            return result;
        }
    }
}
