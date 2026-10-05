using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Tags.Dto;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Tags.Queries.GetTagById
{
    public class GetTagByIdHandler(IUnitOfWork unitOfWork, ILogger<GetTagByIdHandler> logger) : IRequestHandler<GetTagByIdQuery, TagDetailsDto?>
    {
        public async Task<TagDetailsDto?> Handle(GetTagByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get tag operation started.");
            Tag? tag = await unitOfWork.Tags
               .GetByIdAsync(request.TagId);
            var tagDetails = new TagDetailsDto();
            if (tag != null)
            {
                tagDetails.TagName = tag.Name;
                tagDetails.CategoryId = tag.CategoryId;
                tagDetails.TagId = tag.Id;
            }
            logger.LogInformation("Get tag operation started.");
            return tagDetails;
        }
    }
}
