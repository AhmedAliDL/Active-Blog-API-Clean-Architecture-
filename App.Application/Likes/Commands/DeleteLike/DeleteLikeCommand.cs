using MediatR;

namespace App.Application.Likes.Commands.DeleteLike
{
    public record DeleteLikeCommand(Guid BlogId) : IRequest;
}
