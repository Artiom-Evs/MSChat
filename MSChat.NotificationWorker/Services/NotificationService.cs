using MSChat.Shared.Contracts;
using MSChat.Shared.Events;

namespace MSChat.NotificationWorker.Services;

public class NotificationService : INotificationService
{
    private readonly Shared.Contracts.ChatAPI.ChatAPIClient _chatApiClient;
    private readonly PresenceAPI.PresenceAPIClient _presenceApiClient;
    private readonly IAuthAPIClient _authApiClient;
    private readonly IEmailService _emailService;
    private ILogger<NotificationService> _logger;

    public NotificationService(ChatAPI.ChatAPIClient chatApiClient, PresenceAPI.PresenceAPIClient presenceApiClient, IAuthAPIClient authApiClient, IEmailService emailService, ILogger<NotificationService> logger)
    {
        _chatApiClient = chatApiClient;
        _presenceApiClient = presenceApiClient;
        _authApiClient = authApiClient;
        _emailService = emailService;
        _logger = logger;
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
            if (messageSent.SenderId == membership.UserId)
                continue;
            if (membership.LastReadMessageId >= messageSent.MessageId)
                continue;

            var status = usersStatusResponse.Users.FirstOrDefault(us => us.UserId == membership.UserId)?.Status;

            if (status == "online")
                continue;

            var userInfo = await _authApiClient.GetUserInfoAsync(membership.UserId);

            if (userInfo == null)
            {
                _logger.LogWarning("failed to get info about user with ID {0}", membership.UserId);
                continue;
            }

            await _emailService.SendUnreadMessageNotificationAsync(new UnreadMessageInfo()
            {
                UserEmail = userInfo.Email,
                UserName = userInfo.Name,
                ChatName = membership.ChatName,
                MessageSentAt = messageSent.CreatedAt,
            });
            _logger.LogInformation("Notification about unread message sent to user with ID {0}", membership.UserId);
        }
    }
}
