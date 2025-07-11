using Microsoft.AspNetCore.Authorization;

namespace MSChat.ChatAPI.Requirements;

public class ScopeRequirement : IAuthorizationRequirement
{
    public ScopeRequirement(string scope) =>
        Scope = scope;

    public string Scope { get; }
}