using MassTransit;
using MSChat.NotificationWorker.Services;
using MSChat.Shared.Events;

namespace MSChat.NotificationWorker.Consumers;

public class ChatMessageSentConsumer : IConsumer<ChatMessageSent>
{
    private readonly INotificationService _notificationService;

    public ChatMessageSentConsumer(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Consume(ConsumeContext<ChatMessageSent> context)
    {
        // TODO: refactor to persistent storage instead task sleep
        await Task.Delay(TimeSpan.FromMinutes(5));

        await _notificationService.NotifyUsersAsync(context.Message);
    }
}
