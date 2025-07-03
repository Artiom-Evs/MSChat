using Microsoft.AspNetCore.Authorization;
using MSChat.Chat.Requirements;
using System.Security.Claims;

namespace MSChat.Chat.Handlers;

public class ScopeHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScopeRequirement requirement)
    {
        var scopes = context.User.FindFirstValue("scope")?.Split(" ") ?? [];

        if (scopes.Contains(requirement.Scope))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}