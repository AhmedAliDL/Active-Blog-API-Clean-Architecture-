using App.Application.Categories.Dto;
using MediatR;

namespace App.Application.Categories.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDetailsDto?>;
}
