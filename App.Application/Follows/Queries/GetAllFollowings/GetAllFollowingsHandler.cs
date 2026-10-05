using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Follows.Dto;
using MediatR;
using Microsoft.Extensions.Logging;

namespace App.Application.Follows.Queries.GetAllFollowings
{
    public class GetAllFollowingsHandler(IUnitOfWork unitOfWork, IIdentityService identityService, ILogger<GetAllFollowingsHandler> logger) : IRequestHandler<GetAllFollowingsQuery, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(GetAllFollowingsQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Get all followings operation started.");
            List<Guid> bloggerIds = [.. (await unitOfWork.Follows.GetAllAsync(cancellationToken)).Select(f => f.BloggerId)];
            List<UserDto> result = [.. (await identityService.GetUsersByIdsAsync(bloggerIds))
                .Select(u => new UserDto
                {
                    UserName = $"{u.FName} {u.LName}",
                    UserImage = u.Image!
                })];
            logger.LogInformation("Get all followings operation completed.");
            return result;
        }
    }
}
