namespace MSChat.PresenceAPI.Services;

public interface IUserPresenceService
{
    Task SetPresenceStatusAsync(string userId, string status);
    Task<string> GetPresenceStatusAsync(string userId);
}
