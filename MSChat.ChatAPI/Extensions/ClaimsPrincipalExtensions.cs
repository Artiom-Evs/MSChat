using System.Security.Claims;

namespace MSChat.ChatAPI.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return userId;
    }
}
