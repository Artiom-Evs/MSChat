
using StackExchange.Redis;

namespace MSChat.PresenceAPI.Services;

public class UserPresenceService : IUserPresenceService
{
    private readonly IDatabase _redis;

    public UserPresenceService(IConnectionMultiplexer connectionMultiplexer)
    {
        _redis = connectionMultiplexer.GetDatabase();
    }


    public async Task<string> GetPresenceStatusAsync(string userId)
    {
        var statusKey = GetUserStatusKey(userId);
        var redisValue = await _redis.StringGetAsync(statusKey);
        var status = redisValue.HasValue
            ? redisValue.ToString()
            : PresenceStatus.offline;

        return status;        
    }

    public async Task SetPresenceStatusAsync(string userId, string status)
    {
        var statusKey = GetUserStatusKey(userId);
        await _redis.StringSetAsync(statusKey, status, TimeSpan.FromSeconds(10));
    }

    private string GetUserStatusKey(string userId) =>
        $"user:{userId}:status";
}
