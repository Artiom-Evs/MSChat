using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Extensions;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Services;
using System.Net;

namespace MSChat.ChatAPI.Controllers;

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
        var userId = HttpContext.User.GetUserId();
        
        try
        {
            var participants = await _participantsService.GetChatParticipantsAsync(userId, chatId);
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

        var userId = HttpContext.User.GetUserId();

        try
        {
            var participant = await _participantsService.AddParticipantAsync(userId, chatId, addParticipantDto);
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
    [HttpPut("{participantUserId}/role")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> UpdateParticipantRole(long chatId, string participantUserId, UpdateParticipantRoleDto updateRoleDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = HttpContext.User.GetUserId();

        try
        {
            await _participantsService.UpdateParticipantRoleAsync(userId, chatId, participantUserId, updateRoleDto);
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
    /// <param name="participantUserId">The participant member ID to remove</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{participantUserId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> RemoveParticipant(long chatId, string participantUserId)
    {
        var userId = HttpContext.User.GetUserId();

        try
        {
            await _participantsService.RemoveParticipantAsync(userId, chatId, participantUserId);
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
        var userId = HttpContext.User.GetUserId();

        try
        {
            await _participantsService.JoinChatAsync(userId, chatId);
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
        var userId = HttpContext.User.GetUserId();

        try
        {
            await _participantsService.LeaveChatAsync(userId, chatId);
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
}