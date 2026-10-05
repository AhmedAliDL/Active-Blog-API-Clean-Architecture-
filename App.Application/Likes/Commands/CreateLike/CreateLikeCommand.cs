using MediatR;

namespace App.Application.Likes.Commands.CreateLike
{
    public record CreateLikeCommand(Guid BlogId) : IRequest;

}

