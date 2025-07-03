using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSChat.Chat.Data;
using MSChat.Chat.Models;
using MSChat.Chat.Models.DTOs;
using MSChat.Chat.Services;

namespace MSChat.Chat.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatsService _chatsService;

    public ChatsController(ApplicationDbContext context, IChatsService chatsService)
    {
        _dbContext = context;
        _chatsService = chatsService;
    }

    /// <summary>
    /// Get all chats for the current user
    /// </summary>
    /// <returns>List of chats where the user is a member</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChatDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<IEnumerable<ChatDto>>> GetChat()
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);
        var chats = await _chatsService.GetChatsAsync(member.Id);
        return Ok(chats);
    }

    /// <summary>
    /// Get a specific chat by ID
    /// </summary>
    /// <param name="id">The chat ID</param>
    /// <param name="context">HTTP context</param>
    /// <returns>The chat details if found and user is a member</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ChatDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<ChatDto?>> GetChat(long id)
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);
        var chat = await _chatsService.GetChatByIdAsync(member.Id, id);
        
        if (chat == null)
        {
            return NotFound();
        }

        return chat;
    }

    /// <summary>
    /// Update a chat
    /// </summary>
    /// <param name="id">The chat ID</param>
    /// <param name="updateChatDto">The updated chat data</param>
    /// <param name="context">HTTP context</param>
    /// <returns>No content if successful</returns>
    [HttpPut("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> PutChat(long id, UpdateChatDto updateChatDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            await _chatsService.UpdateChatAsync(member.Id, id, updateChatDto);
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
    }

    /// <summary>
    /// Create a new chat
    /// </summary>
    /// <param name="createChatDto">The chat data to create</param>
    /// <param name="context">HTTP context</param>
    /// <returns>The created chat</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ChatDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<ChatDto>> PostChat(CreateChatDto createChatDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Additional validation for personal chats
        if (createChatDto.Type == ChatType.Personal && !createChatDto.OtherMemberId.HasValue)
        {
            ModelState.AddModelError(nameof(createChatDto.OtherMemberId), "OtherMemberId is required for personal chats");
            return BadRequest(ModelState);
        }

        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            var chatDto = await _chatsService.CreateChatAsync(member.Id, createChatDto);
            return CreatedAtAction("GetChat", new { id = chatDto.Id }, chatDto);
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
    /// Delete a chat
    /// </summary>
    /// <param name="id">The chat ID</param>
    /// <param name="context">HTTP context</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> DeleteChat(long id)
    {
        var member = await GetOrCreateChatMemberAsync(HttpContext);

        try
        {
            await _chatsService.DeleteChatAsync(member.Id, id);
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
