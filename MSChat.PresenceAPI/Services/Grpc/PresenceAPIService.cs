using Grpc.Core;
using MSChat.Shared.Contracts;

namespace MSChat.PresenceAPI.Services.Grpc;

public class PresenceAPIService : Shared.Contracts.PresenceAPI.PresenceAPIBase
{
    private readonly IUserPresenceService _presenceService;

    public PresenceAPIService(IUserPresenceService presenceService)
    {
        _presenceService = presenceService;
    }

    public override async Task<GetUsersStatusResponse> GetUsersStatus(GetUsersStatusRequest request, ServerCallContext context)
    {
        var response = new GetUsersStatusResponse();

        foreach (var userId in request.UserIds)
        {
            var status = await _presenceService.GetPresenceStatusAsync(userId);
            response.Users.Add(new UserStatus()
            {
                UserId = userId,
                Status = status,
            });
        }

        return response;
    }
}
