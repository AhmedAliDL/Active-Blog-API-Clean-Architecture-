using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Likes.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Likes.Queries.GetAllBlogLikes
{
    public class GetAllBlogLikesHandler(IUnitOfWork unitOfWork, ILogger<GetAllBlogLikesHandler> logger) : IRequestHandler<GetAllBlogLikesQuery, List<LikeDto>>
    {
        public async Task<List<LikeDto>> Handle(GetAllBlogLikesQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all blog likes operation started.");
            List<LikeDto> result = [.. (await unitOfWork.Likes.FindAllAsync(bl => bl.BlogId == request.BlogId, cancellationToken))
                .Select(l => new LikeDto{
                    BlogId = l.BlogId,
                    LikeId = l.Id,
                    UserId = l.UserId,
                    CreatedAt = l.CreatedAt,
                })];
            logger.LogInformation("Get all blog likes operation completed.");
            return result;
        }
    }
}
