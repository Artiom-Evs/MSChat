using System.Net;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Commands;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Models;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Queries;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/chats/{chatId}/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ISender _mediatr;
    
    public MessagesController(ApplicationDbContext dbContext, ISender commandBus)
    {
        _dbContext = dbContext;
        _mediatr = commandBus;
    }



    /// <summary>
    /// Get messages for a specific chat with pagination
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Number of messages per page (default: 50, max: 100)</param>
    /// <returns>List of messages for the chat</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(
        long chatId, 
        [FromQuery] int limit= 50, 
        [FromQuery] long? offset = null)
    {
        if (limit < 1 || limit > 100)
        {
            ModelState.AddModelError("InvalidQueryParam", "\"limit\" should be in range between 1 and 100.");
            return BadRequest(ModelState);
        }
        else if (offset < 1)
        {
            ModelState.AddModelError("InvalidQueryParam", "\"offset\" should be more then 0.");
            return BadRequest(ModelState);
        }

            var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            var query = new GetMessagesQuery(member.Id, chatId, limit, offset);
            var messages = await _mediatr.Send(query);
            return Ok(messages);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Get a specific message by ID
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <param name="messageId">The message ID</param>
    /// <returns>The message details if found and user has access</returns>
    [HttpGet("{messageId}")]
    [ProducesResponseType(typeof(MessageDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    public async Task<ActionResult<MessageDto>> GetMessage(long chatId, long messageId)
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            var query = new GetMessageQuery(member.Id, chatId, messageId);
            var message = await _mediatr.Send(query);
            
            if (message == null)
            {
                return NotFound();
            }

            // Verify the message belongs to the specified chat
            if (message.ChatId != chatId)
            {
                return NotFound();
            }

            return Ok(message);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Create a new message in the chat
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <param name="createMessageDto">The message data</param>
    /// <returns>The created message</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<MessageDto>> CreateMessage(long chatId, CreateMessageDto createMessageDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            var cmd = new SendMessageCommand(member.Id, chatId, createMessageDto);
            var message = await _mediatr.Send(cmd);
            return CreatedAtAction(nameof(GetMessage), new { chatId, messageId = message.Id }, message);
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
    /// Update an existing message
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <param name="messageId">The message ID</param>
    /// <param name="updateMessageDto">The updated message data</param>
    /// <returns>No content if successful</returns>
    [HttpPut("{messageId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateMessage(long chatId, long messageId, UpdateMessageDto updateMessageDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            var cmd = new UpdateMessageCommand(member.Id, chatId, messageId, updateMessageDto);
            await _mediatr.Send(cmd);
            return NoContent();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a message (soft delete)
    /// </summary>
    /// <param name="chatId">The chat ID</param>
    /// <param name="messageId">The message ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{messageId}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteMessage(long chatId, long messageId)
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            var cmd = new DeleteMessageCommand(member.Id, chatId, messageId);
            await _mediatr.Send(cmd);
            return NoContent();
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