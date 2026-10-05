using MediatR;

namespace App.Application.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid CategoryId, string CategoryName) : IRequest<int>;
}
