using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Tags.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Tags.Queries.GetAllTagsOfCategory
{
    public class GetAllTagsOfCategoryHandler(IUnitOfWork unitOfWork, ILogger<GetAllTagsOfCategoryHandler> logger) : IRequestHandler<GetAllTagsOfCategoryQuery, List<TagDetailsDto>>
    {
        public async Task<List<TagDetailsDto>> Handle(GetAllTagsOfCategoryQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all tags of category operation started.");
            List<TagDetailsDto> result = [.. (await unitOfWork.Tags.FindAllAsync(t => t.CategoryId == request.CatId, cancellationToken))
                .Select(c => new TagDetailsDto{
                    CategoryId = request.CatId,
                    TagId = c.Id,
                    TagName = c.Name
                })];
            logger.LogInformation("Get all tags of category operation completed.");
            return result;
        }
    }
}
