using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Comments.Command.CreateComment
{
    public class CreateCommentHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<CreateCommentHandler> logger, INotificationService notificationService) : IRequestHandler<CreateCommentCommand, int>
    {
        public async Task<int> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Create comment operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty) throw new NotFoundException("User not found");
            if (request.ParentCommentId is not null && request.ParentCommentId != Guid.Empty)
                _ = await unitOfWork.Comments.GetByIdAsync(request.ParentCommentId!.Value) ?? throw new NotFoundException("Parent comment not found");

            var comment = new Comment
            {
                CommentContent = request.CommentContent,
                UserId = userId!.Value,
                BlogId = request.BlogId,
                ParentCommentId = request.ParentCommentId
            };

            await unitOfWork.Comments.AddAsync(comment, cancellationToken);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            if (result > 0)
            {
                if (comment.ParentComment is not null && comment.ParentCommentId != Guid.Empty)
                {
                    var parentComment = await unitOfWork.Comments.GetByIdAsync(comment.ParentCommentId!.Value);
                    if (parentComment is not null)
                        await notificationService.NotifyUserAsync(parentComment!.UserId, comment.CommentContent, comment.UserId, $"api/comments/{comment.Id}");
                }
            }
            logger.LogInformation("Create comment operation completed.");
            return result;
        }
    }
}
