using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Data;
using StackExchange.Redis;
using System.Threading;

namespace MSChat.ChatAPI.Services;

public class ChatMessageIdService : IChatMessageIdService
{
    private readonly IDatabase _redis;
    private readonly ApplicationDbContext _db;

    public ChatMessageIdService(IConnectionMultiplexer connMultiplexer, ApplicationDbContext db)
    {
        _redis = connMultiplexer.GetDatabase();
        _db = db;
    }

    public async Task<long> GetNextIdInChatAsync(long chatId, CancellationToken cancellationToken)
    {
        var lastIdInChatKey = GetLastIdInChatKey(chatId);

        if (!await _redis.KeyExistsAsync(lastIdInChatKey))
        {
            await InitLastIdInChatAsync(chatId, cancellationToken);
        }

        return await _redis.StringIncrementAsync(lastIdInChatKey);
    }

    public async Task<long> GetLastIdInChatAsync(long chatId, CancellationToken cancellationToken = default)
    {
        var lastIdInChatKey = GetLastIdInChatKey(chatId);

        if (!await _redis.KeyExistsAsync(lastIdInChatKey))
        {
            await InitLastIdInChatAsync(chatId, cancellationToken);
        }

        var redisValue = await _redis.StringGetAsync(lastIdInChatKey);

        // received value always should be a correct long number
        if (redisValue.HasValue && long.TryParse(redisValue, out long lastId))
            return lastId;
        throw new InvalidOperationException("Invalid chat last message ID received from the Redis database.");
    }

    private async Task InitLastIdInChatAsync(long chatId, CancellationToken cancellationToken)
    {
        var lastIdInChatKey = GetLastIdInChatKey(chatId);
        var initLockKey = $"{lastIdInChatKey}:initLock";

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

    private string GetLastIdInChatKey(long chatId) => $"chat:{chatId}:lastMessageId";
}
