using MediatR;

namespace App.Application.Categories.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(Guid CategoryId) : IRequest<int>;
}
