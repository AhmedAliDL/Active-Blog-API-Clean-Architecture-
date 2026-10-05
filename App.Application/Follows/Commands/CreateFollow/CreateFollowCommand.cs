using MediatR;

namespace App.Application.Follows.Commands.CreateFollow
{
    public record CreateFollowCommand(Guid BloggerId) : IRequest;
}
