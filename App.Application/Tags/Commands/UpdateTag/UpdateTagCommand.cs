using MediatR;

namespace App.Application.Tags.Commands.UpdateTag
{
    public record UpdateTagCommand(Guid TagId, Guid CategoryId, string? TagName) : IRequest<int>;
}
