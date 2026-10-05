using App.Application.Comments.Dto;
using MediatR;

namespace App.Application.Comments.Queries.GetCommentById
{
    public record GetCommentByIdQuery(Guid CommentId) : IRequest<GetCommentDto>;
}
