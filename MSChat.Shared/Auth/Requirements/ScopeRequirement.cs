using Microsoft.AspNetCore.Authorization;

namespace MSChat.Shared.Auth.Requirements;

public class ScopeRequirement : IAuthorizationRequirement
{
    public ScopeRequirement(string scope) =>
        Scope = scope;

    public string Scope { get; }
}