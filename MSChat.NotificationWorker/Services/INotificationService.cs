using MSChat.Shared.Events;

namespace MSChat.NotificationWorker.Services;

public interface INotificationService
{
    ValueTask NotifyUsersAsync(ChatMessageSent messageSent, CancellationToken cancellationToken = default);
}
