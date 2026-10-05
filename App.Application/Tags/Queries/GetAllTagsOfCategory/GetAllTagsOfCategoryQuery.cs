using App.Application.Tags.Dto;
using MediatR;

namespace App.Application.Tags.Queries.GetAllTagsOfCategory
{
    public record GetAllTagsOfCategoryQuery(Guid CatId) : IRequest<List<TagDetailsDto>>;
}
