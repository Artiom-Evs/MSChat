using GraphQL;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Users;

namespace MSChat.Auth.Controllers;

public class UserInfoDto
{
    public required string UserId { get; init; }
    public required string Name { get; init; }
    public required string Email { get; init; }
}

[ApiController]
[Route("api/users")]
public class UserInfoController : ControllerBase
{
    private readonly UserManager<IUser> _userManager;

    public UserInfoController(UserManager<IUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("{userId}/info")]
    public async Task<IActionResult> GetUserInfoAsync(string userId)
    {
        var claimsPrincipal = HttpContext.User;

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var email = await _userManager.GetEmailAsync(user);


        return Ok(new UserInfoDto
        {
            UserId = userId,
            Name = user.UserName,
            Email = email ?? "",
        });
    }
}
