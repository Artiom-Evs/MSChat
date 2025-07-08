using Microsoft.EntityFrameworkCore;
using MSChat.Chat.Data;
using StackExchange.Redis;

namespace MSChat.Chat.Services;

public class ChatMessageIdGenerator : IChatMessageIdGenerator
{
    private readonly IDatabase _redis;
    private readonly ApplicationDbContext _db;

    public ChatMessageIdGenerator(IConnectionMultiplexer connMultiplexer, ApplicationDbContext db)
    {
        _redis = connMultiplexer.GetDatabase();
        _db = db;
    }


    public async Task<long> GetNextIdInChatAsync(long chatId, CancellationToken cancellationToken)
    {
        var lastIdInChatKey = $"chat:{chatId}:lastMessageId";
        var initLockKey = $"{lastIdInChatKey}:initLock";

        if (!await _redis.KeyExistsAsync(lastIdInChatKey))
        {
            bool isLockTaken = await _redis.StringSetAsync(
                initLockKey,
                "1",
                TimeSpan.FromSeconds(30),
                when: When.NotExists);

            if (isLockTaken)
            {
                try
                {
                    long lastIdInChat = await _db.Messages
                        .Where(m => m.ChatId == chatId)
                        .OrderBy(m => m.Id)
                        .MaxAsync(m => (long?)m.IdInChat, cancellationToken)
                        ?? 0;

                    await _redis.StringGetSetAsync(lastIdInChatKey, lastIdInChat);
                }
                finally
                {
                    await _redis.KeyDeleteAsync(initLockKey);
                }
            }
            else
            {
                while (!await _redis.KeyExistsAsync(lastIdInChatKey))
                {
                    await Task.Delay(50);
                }
            }
        }

        long nextIdInChat = await _redis.StringIncrementAsync(lastIdInChatKey);

        return nextIdInChat;
    }
}
