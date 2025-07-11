using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSChat.Chat.Data;
using MSChat.Chat.Models;
using MSChat.Chat.Models.DTOs;
using MSChat.Chat.Services;

namespace MSChat.Chat.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/chats/{chatId}/participants")]
public class ChatParticipantsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatParticipantsService _participantsService;
    private readonly ILogger<ChatParticipantsController> _logger;

    public ChatParticipantsController(
        ApplicationDbContext dbContext,
        IChatParticipantsService participantsService,
        ILogger<ChatParticipantsController> logger)
    {
        _dbContext = dbContext;
        _participantsService = participantsService;
        _logger = logger;
    }

    /// <summary>
    /// Get all participants in a chat
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <returns>List of chat participants</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChatParticipantDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<IEnumerable<ChatParticipantDto>>> GetChatParticipants(long chatId)
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            var participants = await _participantsService.GetChatParticipantsAsync(member.Id, chatId);
            return Ok(participants);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Add a participant to a chat
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <param name="addParticipantDto">Participant details to add</param>
    /// <returns>The added participant details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ChatParticipantDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<ChatParticipantDto>> AddParticipant(long chatId, AddParticipantDto addParticipantDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            var participant = await _participantsService.AddParticipantAsync(member.Id, chatId, addParticipantDto);
            return CreatedAtAction(nameof(GetChatParticipants), new { chatId }, participant);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Conflict(ModelState);
        }
    }

    /// <summary>
    /// Update a participant's role in a chat
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <param name="participantMemberId">The participant member ID</param>
    /// <param name="updateRoleDto">New role details</param>
    /// <returns>No content if successful</returns>
    [HttpPut("{participantMemberId}/role")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateParticipantRole(long chatId, long participantMemberId, UpdateParticipantRoleDto updateRoleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            await _participantsService.UpdateParticipantRoleAsync(member.Id, chatId, participantMemberId, updateRoleDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
    }

    /// <summary>
    /// Remove a participant from a chat
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <param name="participantMemberId">The participant member ID to remove</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{participantMemberId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> RemoveParticipant(long chatId, long participantMemberId)
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            await _participantsService.RemoveParticipantAsync(member.Id, chatId, participantMemberId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
    }

    /// <summary>
    /// Join a public chat
    /// </summary>
    /// <param name="chatId">The chat ID to join</param>
    /// <returns>No content if successful</returns>
    [HttpPost("join")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> JoinChat(long chatId)
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            await _participantsService.JoinChatAsync(member.Id, chatId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return Conflict(ModelState);
        }
    }

    /// <summary>
    /// Leave a chat
    /// </summary>
    /// <param name="chatId">The chat ID to leave</param>
    /// <returns>No content if successful</returns>
    [HttpPost("leave")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> LeaveChat(long chatId)
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            await _participantsService.LeaveChatAsync(member.Id, chatId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
        }
    }

    private async Task<ChatMember> GetOrCreateChatMemberAsync(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var member = await _dbContext.Members
            .FirstOrDefaultAsync(m => m.UserId == userId);

        if (member == null)
        {
            member = new ChatMember
            {
                UserId = userId,
                Name = context.User.FindFirstValue(ClaimTypes.Name) ?? "Unknown",
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Members.Add(member);
            await _dbContext.SaveChangesAsync();
        }

        return member;
    }
}