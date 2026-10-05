using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Follows.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Follows.Queries.GetAllFollowers
{
    public class GetAllFollowersHandler(IUnitOfWork unitOfWork, IIdentityService identityService, ILogger<GetAllFollowersHandler> logger) : IRequestHandler<GetAllFollowersQuery, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(GetAllFollowersQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all followers operation started.");
            List<Guid> followersIds = [.. (await unitOfWork.Follows.GetAllAsync(cancellationToken)).Select(f => f.FollowerId)];
            List<UserDto> result = [.. (await identityService.GetUsersByIdsAsync(followersIds))
                .Select(u => new UserDto
                {
                    UserName = $"{u.FName} {u.LName}",
                    UserImage = u.Image!
                })];
            logger.LogInformation("Get all followers operation completed.");
            return result;
        }
    }
}
