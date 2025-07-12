using Microsoft.AspNetCore.SignalR;
using MSChat.PresenceAPI.Services;

namespace MSChat.PresenceAPI.Hubs;

public interface IPresenceHubClient
{
    Task PresenceStatusChanged(string userId, string newStatus);
}

public class PresenceHub : Hub<IPresenceHubClient>
{
    private readonly IUserPresenceService _presenceService;

    public PresenceHub(IUserPresenceService presenceService)
    {
        _presenceService = presenceService;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier!;
        var groupName = GetUserStatusSubscriptionName(userId);
        await _presenceService.SetPresenceStatusAsync(userId, PresenceStatus.Online);
        await Clients.Group(groupName).PresenceStatusChanged(userId, PresenceStatus.Online);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier!;
        var groupName = GetUserStatusSubscriptionName(userId);
        await _presenceService.SetPresenceStatusAsync(userId, PresenceStatus.offline);
        await Clients.Group(groupName).PresenceStatusChanged(userId, PresenceStatus.offline);
    }

    public async Task<string> GetUserStatus(string userId)
    {
        return await _presenceService.GetPresenceStatusAsync(userId);
    }

    public async Task SubscribeToUserStatus(string userId)
    {
        var groupName = GetUserStatusSubscriptionName(userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task UnsubscribeFromUserStatus(string userId)
    {
        var groupName = GetUserStatusSubscriptionName(userId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    private string GetUserStatusSubscriptionName(string userId) =>
        $"user:{userId}";
}
