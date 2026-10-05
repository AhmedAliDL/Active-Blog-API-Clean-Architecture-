using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Comments.Command.DeleteComment
{
    public class DeleteCommentHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, ILogger<DeleteCommentHandler> logger) : IRequestHandler<DeleteCommentCommand, int>
    {
        public async Task<int> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Delete comment operation started.");
            Guid? userId = currentUserService.UserId;
            if (userId is null || userId == Guid.Empty) throw new NotFoundException("User not found.");
            Comment comment = await unitOfWork.Comments.GetByIdAsync(request.CommentId) ?? throw new NotFoundException("Comment Not Found.");
            if (comment.UserId == userId) throw new ForbiddenException("You don`t have permission to delete this comment.");

            unitOfWork.Comments.Delete(comment);
            var result = await unitOfWork.CompleteAsync(cancellationToken);
            logger.LogInformation("Delete comment operation completed.");
            return result;
        }
    }
}
