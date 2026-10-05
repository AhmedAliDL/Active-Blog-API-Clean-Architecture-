using App.Application.Follows.Dto;
using MediatR;

namespace App.Application.Follows.Queries.GetAllFollowings
{
    public record GetAllFollowingsQuery : IRequest<List<UserDto>>;
}
