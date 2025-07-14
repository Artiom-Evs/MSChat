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
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Extensions;
using MSChat.ChatAPI.Models;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Controllers;

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
    /// Get all chats for the current user with optional search functionality
    /// </summary>
    /// <param name="search">Optional search term to filter chats by name</param>
    /// <returns>List of chats where the user is a member</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChatDto>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<IEnumerable<ChatDto>>> GetChat([FromQuery] string? search = null)
    {
        var userId = HttpContext.User.GetUserId();
        var chats = await _chatsService.GetChatsAsync(userId, search);
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
        var userId = HttpContext.User.GetUserId();
        var chat = await _chatsService.GetChatByIdAsync(userId, id);
        
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

        var userId = HttpContext.User.GetUserId();

        try
        {
            await _chatsService.UpdateChatAsync(userId, id, updateChatDto);
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
        if (createChatDto.Type == ChatType.Personal && string.IsNullOrEmpty(createChatDto.OtherMemberId))
        {
            ModelState.AddModelError(nameof(createChatDto.OtherMemberId), "OtherMemberId is required for personal chats");
            return BadRequest(ModelState);
        }

        var userId = HttpContext.User.GetUserId();

        try
        {
            var chatDto = await _chatsService.CreateChatAsync(userId, createChatDto);
            return CreatedAtAction("GetChat", new { id = chatDto.Id }, chatDto);
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return BadRequest(ModelState);
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
        var userId = HttpContext.User.GetUserId();

        try
        {
            await _chatsService.DeleteChatAsync(userId, id);
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
}
