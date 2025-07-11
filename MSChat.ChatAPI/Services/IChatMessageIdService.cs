namespace MSChat.ChatAPI.Services;


public interface IChatMessageIdService
{
    Task<long> GetNextIdInChatAsync(long chatId, CancellationToken cancellationToken = default);
    Task<long> GetLastIdInChatAsync(long chatId, CancellationToken cancellationToken = default);
}
