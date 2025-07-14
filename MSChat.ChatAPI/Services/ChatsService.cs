using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Models;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public class ChatsService : IChatsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatMessageIdService _chatIdService;
    private readonly ILogger<ChatsService> _logger;

    public ChatsService(ApplicationDbContext dbContext, IChatMessageIdService chatIdService, ILogger<ChatsService> logger)
    {
        _dbContext = dbContext;
        _chatIdService = chatIdService;
        _logger = logger;
    }

    public async Task<IEnumerable<ChatDto>> GetChatsAsync(string userId, string? search = null)
    {
        var query = _dbContext.Chat
            .Where(c => c.Type == ChatType.Public  || c.Members.Any(m => m.Member.UserId == userId));

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(c => c.Name.Contains(search));
        }

        var chats = await query
            .Select(c => new ChatDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                CreatedAt = c.CreatedAt,
                DeletedAt = c.DeletedAt,
                IsInChat = c.Members.Any(m => m.Member.UserId == userId),
                Participants = c.Members
                    .Take(2)
                    .Select(cm => new ChatParticipantDto
                    {
                        ChatId = c.Id,
                        MemberId = cm.Member.UserId,
                        MemberName = cm.Member.Name,
                        MemberPhotoUrl = cm.Member.PhotoUrl,
                        RoleInChat = cm.RoleInChat,
                        JoinedAt = cm.JoinedAt
                    })
                    .ToList(),
                LastMessage = _dbContext.Messages
                    .Where(m => m.ChatId == c.Id && m.DeletedAt == null)
                    .OrderByDescending(m => m.CreatedAt)
                    .Join(_dbContext.Members,
                          m => m.SenderId,
                          member => member.Id,
                          (m, member) => new MessageDto
                          {
                              Id = m.Id,
                              ChatId = m.ChatId,
                              SenderId = member.UserId,
                              SenderName = member.Name,
                              SenderPhotoUrl = member.PhotoUrl,
                              Text = m.Text,
                              CreatedAt = m.CreatedAt,
                              UpdatedAt = m.UpdatedAt,
                              DeletedAt = m.DeletedAt
                          })
                    .FirstOrDefault()
            })
            .ToListAsync();

        var chatMemberships = await _dbContext.ChatMemberships
            .Include(m => m.Chat)
            .Include(m => m.Member)
            .Where(m => m.Member.UserId == userId)
            .ToListAsync();

        foreach (var chat in chats)
        {
            var chatMembership = chatMemberships.FirstOrDefault(cm => cm.ChatId == chat.Id);

            if (chatMembership != null)
            {
                var lastIdInChat = await _chatIdService.GetLastIdInChatAsync(chat.Id);
                chat.UnreadMessagesCount = (int)(lastIdInChat - chatMembership.LastReadedMessageId);
            }
        }

        return chats;
    }

    public async Task<ChatDto?> GetChatByIdAsync(string userId, long chatId)
    {
        var chat = await _dbContext.Chat
            .Where(c => c.Id == chatId && (c.Type == ChatType.Public || c.Members.Any(m => m.Member.UserId == userId)))
            .Select(c => new ChatDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                CreatedAt = c.CreatedAt,
                DeletedAt = c.DeletedAt,
                IsInChat = c.Members.Any(m => m.Member.UserId == userId),
                Participants = c.Members.Select(cm => new ChatParticipantDto
                {
                    ChatId = c.Id,
                    MemberId = cm.Member.UserId,
                    MemberName = cm.Member.Name,
                    MemberPhotoUrl = cm.Member.PhotoUrl,
                    RoleInChat = cm.RoleInChat,
                    JoinedAt = cm.JoinedAt
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return chat;
    }

    public async Task<ChatDto> CreateChatAsync(string userId, CreateChatDto createChatDto)
    {
        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null)
        {
            throw new ArgumentException("Member not found", nameof(userId));
        }

        if (createChatDto.Type == ChatType.Personal)
        {
            return await CreatePersonalChatAsync(userId, createChatDto);
        }
        else
        {
            return await CreatePublicChatAsync(userId, createChatDto);
        }
    }

    private async Task<ChatDto> CreatePersonalChatAsync(string userId, CreateChatDto createChatDto)
    {
        if (!createChatDto.OtherMemberId.HasValue)
        {
            throw new ArgumentException("OtherMemberId is required for personal chats", nameof(createChatDto.OtherMemberId));
        }

        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null)
        {
            throw new ArgumentException("Member not found", nameof(userId));
        }

        var otherMember = await _dbContext.Members.FindAsync(createChatDto.OtherMemberId.Value);
        if (otherMember == null)
        {
            throw new ArgumentException("Other member not found", nameof(createChatDto.OtherMemberId));
        }

        // Check if a personal chat already exists between these members
        var existingChat = await _dbContext.Chat
            .Where(c => c.Type == ChatType.Personal && 
                       c.Members.Count == 2 &&
                       c.Members.Any(m => m.Member.UserId == userId) &&
                       c.Members.Any(m => m.MemberId == createChatDto.OtherMemberId.Value))
            .FirstOrDefaultAsync();

        // If an existing chat is found, return it with participants
        if (existingChat != null)
        {
            var existingChatDto = await _dbContext.Chat
                .Where(c => c.Id == existingChat.Id)
                .Select(c => new ChatDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Type = c.Type,
                    CreatedAt = c.CreatedAt,
                    DeletedAt = c.DeletedAt,
                    IsInChat = true,
                    Participants = c.Members.Select(cm => new ChatParticipantDto
                    {
                        ChatId = c.Id,
                        MemberId = cm.Member.UserId,
                        MemberName = cm.Member.Name,
                        MemberPhotoUrl = cm.Member.PhotoUrl,
                        RoleInChat = cm.RoleInChat,
                        JoinedAt = cm.JoinedAt
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return existingChatDto!;
        }

        var chat = new Models.Chat
        {
            Name = createChatDto.Name,
            Type = ChatType.Personal,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Chat.Add(chat);
        
        // Add both members to the chat
        _dbContext.ChatMemberships.Add(new ChatMembership
        {
            Chat = chat,
            MemberId = member.Id,
            RoleInChat = ChatRole.Owner,
            JoinedAt = DateTime.UtcNow
        });
        
        _dbContext.ChatMemberships.Add(new ChatMembership
        {
            Chat = chat,
            MemberId = createChatDto.OtherMemberId.Value,
            RoleInChat = ChatRole.Owner,
            JoinedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();

        // Return the created personal chat with participants
        var createdChatDto = await _dbContext.Chat
            .Where(c => c.Id == chat.Id)
            .Select(c => new ChatDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                CreatedAt = c.CreatedAt,
                DeletedAt = c.DeletedAt,
                IsInChat = true,
                Participants = c.Members.Select(cm => new ChatParticipantDto
                {
                    ChatId = c.Id,
                    MemberId = cm.Member.UserId,
                    MemberName = cm.Member.Name,
                    MemberPhotoUrl = cm.Member.PhotoUrl,
                    RoleInChat = cm.RoleInChat,
                    JoinedAt = cm.JoinedAt
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return createdChatDto!;
    }

    private async Task<ChatDto> CreatePublicChatAsync(string userId, CreateChatDto createChatDto)
    {
        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.UserId == userId);
        if (member == null)
        {
            throw new ArgumentException("Member not found", nameof(userId));
        }

        var chat = new Models.Chat
        {
            Name = createChatDto.Name,
            Type = ChatType.Public,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Chat.Add(chat);
        _dbContext.ChatMemberships.Add(new ChatMembership
        {
            Chat = chat,
            MemberId = member.Id,
            RoleInChat = ChatRole.Owner,
            JoinedAt = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();

        // Return the created public chat with participants
        var createdChatDto = await _dbContext.Chat
            .Where(c => c.Id == chat.Id)
            .Select(c => new ChatDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                CreatedAt = c.CreatedAt,
                DeletedAt = c.DeletedAt,
                IsInChat = true,
                Participants = c.Members.Select(cm => new ChatParticipantDto
                {
                    ChatId = c.Id,
                    MemberId = cm.Member.UserId,
                    MemberName = cm.Member.Name,
                    MemberPhotoUrl = cm.Member.PhotoUrl,
                    RoleInChat = cm.RoleInChat,
                    JoinedAt = cm.JoinedAt
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return createdChatDto!;
    }

    public async Task UpdateChatAsync(string userId, long chatId, UpdateChatDto updateChatDto)
    {
        var chat = await _dbContext.Chat
            .FirstOrDefaultAsync(c => c.Id == chatId && c.Members.Any(m => m.Member.UserId == userId));

        if (chat == null)
        {
            throw new KeyNotFoundException("Chat not found or user is not a member");
        }

        var chatMemberLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == userId);

        if (chatMemberLink?.RoleInChat != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only chat owners can update the chat");
        }

        chat.Name = updateChatDto.Name;
        
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ChatExistsAsync(chatId))
            {
                throw new KeyNotFoundException("Chat not found");
            }
            else
            {
                throw;
            }
        }
    }

    public async Task DeleteChatAsync(string userId, long chatId)
    {
        var chat = await _dbContext.Chat
            .FirstOrDefaultAsync(c => c.Id == chatId && c.Members.Any(m => m.Member.UserId == userId));

        if (chat == null)
        {
            throw new KeyNotFoundException("Chat not found or user is not a member");
        }

        var chatMemberLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == userId);

        if (chatMemberLink?.RoleInChat != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only chat owners can delete the chat");
        }

        _dbContext.Chat.Remove(chat);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<bool> ChatExistsAsync(long chatId)
    {
        return await _dbContext.Chat.AnyAsync(e => e.Id == chatId);
    }
}
