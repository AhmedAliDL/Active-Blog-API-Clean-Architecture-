using App.Application.Comments.Dto;
using App.Application.Common.Enums;
using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Comments.Queries.GetAllCommentOfBlog
{
    public class GetCommentsOfBlogHandler(IUnitOfWork unitOfWork, ILogger<GetCommentsOfBlogHandler> logger) : IRequestHandler<GetCommentsOfBlogQuery, List<CommentDto>>
    {
        public async Task<List<CommentDto>> Handle(GetCommentsOfBlogQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get comments of blog operation started.");
            var blogComments = await unitOfWork.Comments.FindAllAsync(criteria: bc => bc.BlogId == request.BlogId, includes: [bc => bc.User], sortDirection: SortDirection.Descending, orderBy: bc => bc.CreatedAt, cancellationToken: cancellationToken) ?? throw new NotFoundException("Blog not found.");

            var commentDtos = blogComments
                .ToDictionary(
                c => c.Id,
                c => new CommentDto
                {
                    UserName = $"{c.User.FName} {c.User.LName}",
                    CommentContent = c.CommentContent,
                    UserImage = c.User.Image,
                    CommentDate = c.CreatedAt
                });
            var rootComments = new List<CommentDto>();

            foreach (var comment in blogComments)
            {
                var dto = commentDtos[comment.Id];

                if (comment.ParentCommentId is null)
                {
                    rootComments.Add(dto);
                }
                else if (commentDtos.TryGetValue(
                             comment.ParentCommentId.Value,
                             out var parent))
                {
                    parent.Replies.Add(dto);
                }
            }
            logger.LogInformation("Get comments of blog operation completed.");
            return rootComments;
        }
    }
}
