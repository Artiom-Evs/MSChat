using Microsoft.AspNetCore.Mvc;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Middlewares;

public class MemberRegistrationMiddleware
{
    private readonly RequestDelegate _next;
    
    public MemberRegistrationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context,  IMemberRegistrationService registrationService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await registrationService.EnsureMemberRegisteredAsync(context.User);
        }

        await _next(context);
    }
}
