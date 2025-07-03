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

namespace MSChat.Chat.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class ChatsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ChatsController(ApplicationDbContext context)
    {
        _dbContext = context;
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

        var chats = await _dbContext.Chat
            .Where(c => c.Members.Any(m => m.MemberId == member.Id))
            .Select(c => new ChatDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                CreatedAt = c.CreatedAt,
                DeletedAt = c.DeletedAt
            })
            .ToListAsync();

        return chats;
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

        var chat = await _dbContext.Chat
            .Where(c => c.Id == id && c.Members.Any(m => m.MemberId == member.Id))
            .Select(c => new ChatDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                CreatedAt = c.CreatedAt,
                DeletedAt = c.DeletedAt
            })
            .FirstOrDefaultAsync();

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

        var existedChat = await _dbContext.Chat
            .FirstOrDefaultAsync(c => c.Id == id && c.Members.Any(m => m.MemberId == member.Id));

        if (existedChat == null)
        {
            return NotFound();
        }

        var chatMemberLink = await _dbContext.ChatMemberLinks
            .FirstAsync(cml => cml.ChatId == existedChat.Id && cml.MemberId == member.Id);

        if (chatMemberLink.RoleInChat != ChatRole.Owner)
        {
            return Forbid();
        }

        existedChat.Name = updateChatDto.Name;
        existedChat.Type = updateChatDto.Type;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ChatExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
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

        var member = await GetOrCreateChatMemberAsync(HttpContext);

        var chat = new Models.Chat
        {
            Name = createChatDto.Name,
            Type = createChatDto.Type,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Chat.Add(chat);
        _dbContext.ChatMemberLinks.Add(new ChatMemberLink()
        {
            Member = member,
            Chat = chat,
            RoleInChat = ChatRole.Owner,
            JoinedAt = DateTime.UtcNow,
        });

        await _dbContext.SaveChangesAsync();

        var chatDto = new ChatDto
        {
            Id = chat.Id,
            Name = chat.Name,
            Type = chat.Type,
            CreatedAt = chat.CreatedAt,
            DeletedAt = chat.DeletedAt
        };

        return CreatedAtAction("GetChat", new { id = chat.Id }, chatDto);
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

        var chat = await _dbContext.Chat
            .FirstOrDefaultAsync(c => c.Id == id && c.Members.Any(cml => cml.MemberId == member.Id));

        if (chat == null)
        {
            return NotFound();
        }


        var chatMemberLink = await _dbContext.ChatMemberLinks
            .FirstAsync(cml => cml.ChatId == chat.Id && cml.MemberId == member.Id);

        if (chatMemberLink.RoleInChat != ChatRole.Owner)
        {
            return Forbid();
        }


        _dbContext.Chat.Remove(chat);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool ChatExists(long id)
    {
        return _dbContext.Chat.Any(e => e.Id == id);
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
