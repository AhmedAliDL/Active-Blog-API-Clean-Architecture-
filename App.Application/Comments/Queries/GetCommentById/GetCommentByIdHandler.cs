using App.Application.Comments.Dto;
using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.UnitOfWork;
using MediatR;

namespace App.Application.Comments.Queries.GetCommentById
{
    public class GetCommentByIdHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCommentByIdQuery, GetCommentDto>
    {
        public async Task<GetCommentDto> Handle(GetCommentByIdQuery request, CancellationToken cancellationToken)
        {
            var comment = await unitOfWork.Comments.GetByIdAsync(request.CommentId) ?? throw new NotFoundException("Comment not found.");
            var commentDto = new GetCommentDto
            {
                BlogId = comment.BlogId,
                CommentContent = comment.CommentContent,
                ParentCommentId = comment.ParentCommentId,
                UserId = comment.UserId
            };
            return commentDto;
        }
    }
}
