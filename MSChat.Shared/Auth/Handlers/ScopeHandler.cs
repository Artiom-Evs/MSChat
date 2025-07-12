using Microsoft.AspNetCore.Authorization;
using MSChat.Shared.Auth.Requirements;

namespace MSChat.Shared.Auth.Handlers;

public class ScopeHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ScopeRequirement requirement)
    {
        var scopes = context.User.FindFirst("scope")
            ?.Value
            ?.Split(" ") ?? [];

        if (scopes.Contains(requirement.Scope))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}