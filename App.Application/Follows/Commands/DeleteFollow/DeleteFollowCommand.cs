using MediatR;

namespace App.Application.Follows.Commands.DeleteFollow
{
    public record DeleteFollowCommand(Guid BloggerId) : IRequest;
}
