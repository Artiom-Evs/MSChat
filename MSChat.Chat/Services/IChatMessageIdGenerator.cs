namespace MSChat.Chat.Services;


public interface IChatMessageIdGenerator
{
    Task<long> GetNextIdInChatAsync(long chatId, CancellationToken cancellationToken = default);
}
