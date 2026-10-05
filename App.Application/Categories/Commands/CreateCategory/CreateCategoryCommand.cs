using MediatR;

namespace App.Application.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string CategoryName) : IRequest<int>;
}
