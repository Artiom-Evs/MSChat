using MSChat.Shared.Contracts;
using MSChat.Shared.Events;

namespace MSChat.NotificationWorker.Services;

public class NotificationService : INotificationService
{
    private readonly Shared.Contracts.ChatAPI.ChatAPIClient _chatApiClient;
    private readonly PresenceAPI.PresenceAPIClient _presenceApiClient;

    public NotificationService(ChatAPI.ChatAPIClient chatApiClient, PresenceAPI.PresenceAPIClient presenceApiClient)
    {
        _chatApiClient = chatApiClient;
        _presenceApiClient = presenceApiClient;
    }

    public async ValueTask NotifyUsersAsync(ChatMessageSent messageSent, CancellationToken cancellationToken = default)
    {
        var membershipsResponse = await _chatApiClient.GetChatMembershipsAsync(new GetChatMembershipsRequest()
        {
            ChatId = messageSent.ChatId,
        });

        var usersStatusRequest = new GetUsersStatusRequest();
        usersStatusRequest.UserIds.AddRange(membershipsResponse.Memberships.Select(m => m.UserId));
        var usersStatusResponse = await _presenceApiClient.GetUsersStatusAsync(usersStatusRequest);

        foreach (var membership in membershipsResponse.Memberships)
        {
            if (membership.LastReadMessageId >= messageSent.MessageId)
                continue;

            var status = usersStatusResponse.Users.FirstOrDefault(us => us.UserId == membership.UserId)?.Status;

            if (status == "online")
                continue;

            Console.WriteLine("Send notification!");
        }
    }
}
