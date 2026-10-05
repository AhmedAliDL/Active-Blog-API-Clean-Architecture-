using MediatR;

namespace App.Application.Tags.Commands.DeleteTag
{
    public record DeleteTagCommand(Guid TagId) : IRequest<int>;
}
