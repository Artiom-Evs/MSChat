using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSChat.Chat.Models.DTOs;
using MSChat.Chat.Services;

namespace MSChat.Chat.Controllers;

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
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MemberDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<MemberDto>> GetMember(long id)
    {
        var member = await _membersService.GetMemberByIdAsync(id);
        
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
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User is not authenticated.");
        }

        var member = await _membersService.GetCurrentMemberAsync(userId);
        
        if (member == null)
        {
            return NotFound("Member not found for the current user.");
        }

        return Ok(member);
    }
}