using System.Security.Claims;

namespace MSChat.ChatAPI.Services;

public interface IMemberRegistrationService
{
    /// <summary>
    /// guarantee that user is registered as a chat member
    /// </summary>
    ValueTask EnsureMemberRegisteredAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
}
