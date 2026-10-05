using App.Application.ContentBlocks.Dto;
using MediatR;

namespace App.Application.ContentBlocks.Queries.GetAllContentBlocks
{
    public record GetAllContentBlocksOfBlogQuery(Guid BlogId) : IRequest<List<ContentBlockDto>>;
}
