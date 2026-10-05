using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Comments.Command.UpdateComment
{
    public class UpdateCommentHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<UpdateCommentHandler> logger) : IRequestHandler<UpdateCommentCommand, int>
    {
        public async Task<int> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Update comment operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty) throw new NotFoundException("User Not Found.");
            Comment oldComment = await unitOfWork.Comments.GetByIdAsync(request.CommentId) ?? throw new NotFoundException("Comment Not Found.");
            if (oldComment.UserId == userId)
                throw new ForbiddenException("You don`t have permission to edit this comment.");

            oldComment.CommentContent = request.CommentContent;
            oldComment.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Comments.Update(oldComment);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Update comment operation completed.");
            return result;
        }
    }
}
