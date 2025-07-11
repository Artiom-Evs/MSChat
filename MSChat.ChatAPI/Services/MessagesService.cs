using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Models;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public class MessagesService : IMessagesService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatMessageIdService _chatIdService;
    private readonly ILogger<MessagesService> _logger;

    public MessagesService(ApplicationDbContext dbContext, IChatMessageIdService idGenerator, ILogger<MessagesService> logger)
    {
        _dbContext = dbContext;
        _chatIdService = idGenerator;
        _logger = logger;
    }

    public async Task<IEnumerable<MessageDto>> GetMessagesAsync(long memberId, long chatId, int limit, long? offset)
    {
        // Verify user is a member of the chat
        var chatMembership = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == memberId);

        if (chatMembership == null)
        {
            // Check if it's a public chat
            var isPublicChat = await _dbContext.Chat
                .AnyAsync(c => c.Id == chatId && c.Type == ChatType.Public);

            if (!isPublicChat)
            {
                throw new UnauthorizedAccessException("User is not a member of this chat");
            }
        }

        var query = _dbContext.Messages
            .Where(m => m.ChatId == chatId);

        // take last messages by limit if offset is null
        if (offset == null)
        {
            query = query
                .OrderByDescending(m => m.Id)
                .Take(limit);
        }
        else
        {
            query = query
                .OrderBy(m => m.Id)
                .Where(m => m.IdInChat > offset)
                .Take(limit);
        }

        var messages = await query
            .Join(_dbContext.Members,
                    m => m.SenderId,
                    member => member.Id,
                    (m, member) => new MessageDto
                    {
                        Id = m.IdInChat,
                        ChatId = m.ChatId,
                        SenderId = m.SenderId,
                        SenderName = member.Name,
                        SenderPhotoUrl = member.PhotoUrl,
                        Text = m.Text,
                        CreatedAt = m.CreatedAt,
                        UpdatedAt = m.UpdatedAt,
                        DeletedAt = m.DeletedAt
                    })
            .ToListAsync();

        if (chatMembership != null && messages.Any())
        {
            var lastMessageId = messages.Max(m => m.Id);
            chatMembership.LastReadedMessageId = lastMessageId;
            await _dbContext.SaveChangesAsync();
        }

        return messages.OrderBy(m => m.CreatedAt);
    }

    public async Task<MessageDto?> GetMessageByIdAsync(long memberId, long chatId, long messageIdInChat)
    {
        // Verify user is a member of the chat
        var chatMembership = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == memberId);

        if (chatMembership == null)
        {
            // Check if it's a public chat
            var isPublicChat = await _dbContext.Chat
                .AnyAsync(c => c.Id == chatId && c.Type == ChatType.Public);

            if (!isPublicChat)
            {
                throw new UnauthorizedAccessException("User is not a member of this chat");
            }
        }

        var message = await _dbContext.Messages
            .Where(m => m.ChatId == chatId && m.IdInChat == messageIdInChat)
            .Join(_dbContext.Members,
                  m => m.SenderId,
                  member => member.Id,
                  (m, member) => new { Message = m, Member = member })
            .FirstOrDefaultAsync();

        if (message == null)
        {
            return null;
        }

        if (chatMembership != null)
        {
            chatMembership.LastReadedMessageId = message.Message.IdInChat;
            await _dbContext.SaveChangesAsync();
        }

        return new MessageDto
        {
            Id = message.Message.IdInChat,
            ChatId = message.Message.ChatId,
            SenderId = message.Message.SenderId,
            SenderName = message.Member.Name,
            SenderPhotoUrl = message.Member.PhotoUrl,
            Text = message.Message.Text,
            CreatedAt = message.Message.CreatedAt,
            UpdatedAt = message.Message.UpdatedAt,
            DeletedAt = message.Message.DeletedAt
        };
    }

    public async Task<MessageDto> CreateMessageAsync(long memberId, long chatId, CreateMessageDto createMessageDto)
    {
        // Verify user is a member of the chat
        var isMember = await _dbContext.ChatMemberships
            .AnyAsync(cml => cml.ChatId == chatId && cml.MemberId == memberId);

        if (!isMember)
        {
            throw new UnauthorizedAccessException("User is not a member of this chat");
        }

        // Verify chat exists and is not deleted
        var chat = await _dbContext.Chat
            .FirstOrDefaultAsync(c => c.Id == chatId && c.DeletedAt == null);

        if (chat == null)
        {
            throw new KeyNotFoundException("Chat not found or has been deleted");
        }

        long idInChat = await _chatIdService.GetNextIdInChatAsync(chatId);

        var message = new Message
        {
            IdInChat = idInChat,
            ChatId = chatId,
            SenderId = memberId,
            Text = createMessageDto.Text,
            CreatedAt = DateTime.UtcNow,
        };

        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();

        // Return the created message with sender information
        var member = await _dbContext.Members.FindAsync(memberId);
        return new MessageDto
        {
            Id = message.IdInChat,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            SenderName = member?.Name ?? "Unknown",
            SenderPhotoUrl = member?.PhotoUrl,
            Text = message.Text,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt,
            DeletedAt = message.DeletedAt
        };
    }

    public async Task UpdateMessageAsync(long memberId, long chatId, long messageIdInChat, UpdateMessageDto updateMessageDto)
    {
        var message = await _dbContext.Messages
            .FirstOrDefaultAsync(m => m.ChatId == chatId && m.IdInChat == messageIdInChat);

        if (message == null)
        {
            throw new KeyNotFoundException("Message not found");
        }

        // Only the sender can update their own message
        if (message.SenderId != memberId)
        {
            throw new UnauthorizedAccessException("You can only update your own messages");
        }

        // Cannot update deleted messages
        if (message.DeletedAt.HasValue)
        {
            throw new InvalidOperationException("Cannot update a deleted message");
        }

        message.Text = updateMessageDto.Text;
        message.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await MessageExistsAsync(message.Id))
            {
                throw new KeyNotFoundException("Message not found");
            }
            else
            {
                throw;
            }
        }
    }

    public async Task DeleteMessageAsync(long memberId, long chatId, long messageIdInChat)
    {
        var message = await _dbContext.Messages
            .FirstOrDefaultAsync(m => m.ChatId == chatId && m.IdInChat == messageIdInChat);

        if (message == null)
        {
            throw new KeyNotFoundException("Message not found");
        }

        // Only the sender can delete their own message, or chat owners can delete any message
        if (message.SenderId != memberId)
        {
            // Check if user is a chat owner
            var isOwner = await _dbContext.ChatMemberships
                .AnyAsync(cml => cml.ChatId == message.ChatId && cml.MemberId == memberId && cml.RoleInChat == ChatRole.Owner);

            if (!isOwner)
            {
                throw new UnauthorizedAccessException("You can only delete your own messages or you must be a chat owner");
            }
        }

        // Soft delete the message
        message.DeletedAt = DateTime.UtcNow;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await MessageExistsAsync(message.Id))
            {
                throw new KeyNotFoundException("Message not found");
            }
            else
            {
                throw;
            }
        }
    }

    private async Task<bool> MessageExistsAsync(long messageId)
    {
        return await _dbContext.Messages.AnyAsync(e => e.Id == messageId);
    }
}