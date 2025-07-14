using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSChat.ChatAPI.Extensions;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class MembersController : ControllerBase
{
    private readonly IMembersService _membersService;
    private readonly ILogger<MembersController> _logger;

    public MembersController(IMembersService membersService, ILogger<MembersController> logger)
    {
        _membersService = membersService;
        _logger = logger;
    }

    /// <summary>
    /// Get all members with optional search functionality
    /// </summary>
    /// <param name="search">Optional search term to filter members by name</param>
    /// <returns>List of members</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MemberDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers([FromQuery] string? search = null)
    {
        var members = await _membersService.GetMembersAsync(search);
        return Ok(members);
    }

    /// <summary>
    /// Get a specific member by ID
    /// </summary>
    /// <param name="id">The member ID</param>
    /// <returns>The member details if found</returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(MemberDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<MemberDto>> GetMember(string userId)
    {
        var member = await _membersService.GetMemberByUserIdAsync(userId);
        
        if (member == null)
        {
            return NotFound();
        }

        return Ok(member);
    }

    /// <summary>
    /// Get information about the current authenticated user
    /// </summary>
    /// <returns>The current user's member information</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(MemberDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<MemberDto>> GetCurrentMember()
    {
        var userId = HttpContext.User.GetUserId();
        var member = await _membersService.GetCurrentMemberAsync(userId);
        
        if (member == null)
        {
            return NotFound("Member not found for the current user.");
        }

        return Ok(member);
    }
}