using MediatR;

namespace App.Application.Tags.Commands.CreateTag
{
    public record CreateTagCommand(Guid CategoryId, string TagName) : IRequest<int>;
}
