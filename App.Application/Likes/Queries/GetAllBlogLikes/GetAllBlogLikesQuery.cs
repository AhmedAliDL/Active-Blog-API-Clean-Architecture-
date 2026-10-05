using App.Application.Likes.Dto;
using MediatR;

namespace App.Application.Likes.Queries.GetAllBlogLikes
{
    public record GetAllBlogLikesQuery(Guid BlogId) : IRequest<List<LikeDto>>;
}
