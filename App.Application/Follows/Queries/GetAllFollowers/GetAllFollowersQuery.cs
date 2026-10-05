using App.Application.Follows.Dto;
using MediatR;

namespace App.Application.Follows.Queries.GetAllFollowers
{
    public record GetAllFollowersQuery : IRequest<List<UserDto>>;
}
