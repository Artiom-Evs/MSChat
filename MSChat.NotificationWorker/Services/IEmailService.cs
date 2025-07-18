namespace MSChat.NotificationWorker.Services;

public record UnreadMessageInfo
{
    public required string UserEmail { get; init; }
    public required string UserName { get; init; }
    public required string ChatName { get; init; }
    public required DateTime MessageSentAt { get; init; }
};

public interface IEmailService
{
    ValueTask SendUnreadMessageNotificationAsync(UnreadMessageInfo messageInfo, CancellationToken cancellationToken = default);
}
