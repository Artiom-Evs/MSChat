using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Extensions;
using MSChat.ChatAPI.Models;
using StackExchange.Redis;
using System.Security.Claims;

namespace MSChat.ChatAPI.Services;

public class MemberRegistrationService : IMemberRegistrationService
{
    private readonly IDatabase _redis;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<MemberRegistrationService> _logger;

    public MemberRegistrationService(IConnectionMultiplexer connectionMultiplexer, ApplicationDbContext dbContext, ILogger<MemberRegistrationService> logger)
    {
        _redis = connectionMultiplexer.GetDatabase();
        _dbContext = dbContext;
        _logger = logger;
    }

    public async ValueTask EnsureMemberRegisteredAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var userId = user.GetUserId();
        var existanceKey = GetMemberExistanceKey(userId);

        if (!await _redis.KeyExistsAsync(existanceKey))
        {
            await RegisterMemberOrWaitAsync(user, cancellationToken);
        }

        // user registered successfully
    }

    private async ValueTask RegisterMemberOrWaitAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
    {
        var userId = user.GetUserId();
        var existanceKey = GetMemberExistanceKey(userId);
        var creationLockKey = $"{existanceKey}:creationLock";

        bool isLockTaken = await _redis.StringSetAsync(
            creationLockKey,
            "1",
            TimeSpan.FromSeconds(30),
            when: When.NotExists);

        if (isLockTaken)
        {
            try
            {
                var member = await _dbContext.Members
                    .FirstOrDefaultAsync(m => m.UserId == userId, cancellationToken);

                if (member == null)
                {
                    member = new ChatMember
                    {
                        UserId = userId,
                        Name = user.FindFirstValue(ClaimTypes.Name) ?? "Unknown",
                        CreatedAt = DateTime.UtcNow
                    };

                    _dbContext.Members.Add(member);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("New member successfully registered. Member ID: {}.", member.Id);
                }

                await _redis.StringSetAsync(existanceKey, "1");
            }
            finally
            {
                await _redis.KeyDeleteAsync(creationLockKey);
            }
        }
        else
        {
            while (!await _redis.KeyExistsAsync(existanceKey))
            {
                await Task.Delay(50);
            }
        }
    }

    private string GetMemberExistanceKey(string userId) => $"user:{userId}:existed";
}
