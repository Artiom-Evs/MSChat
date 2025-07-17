namespace MSChat.NotificationWorker.Services;

public interface IAccessTokenProvider
{
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
