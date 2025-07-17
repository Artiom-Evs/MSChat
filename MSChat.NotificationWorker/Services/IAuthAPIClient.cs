using MSChat.Shared.Auth.Models.Dtos;

namespace MSChat.NotificationWorker.Services;

public interface IAuthAPIClient
{
    Task<UserInfoDto?> GetUserInfoAsync(string userId, CancellationToken cancellationToken = default);
}
