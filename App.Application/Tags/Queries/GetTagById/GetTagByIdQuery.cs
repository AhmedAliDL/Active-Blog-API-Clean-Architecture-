using App.Application.Tags.Dto;
using MediatR;

namespace App.Application.Tags.Queries.GetTagById
{
    public record GetTagByIdQuery(Guid TagId) : IRequest<TagDetailsDto?>;
}
