using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Models;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public class ChatParticipantsService : IChatParticipantsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ChatParticipantsService> _logger;

    public ChatParticipantsService(ApplicationDbContext dbContext, ILogger<ChatParticipantsService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<ChatParticipantDto>> GetChatParticipantsAsync(string requestingUserId, long chatId)
    {
        // Check if requesting member has access to the chat
        var hasAccess = await _dbContext.ChatMemberships
            .AnyAsync(cml => cml.ChatId == chatId && (cml.Chat.Type == ChatType.Public || cml.Member.UserId == requestingUserId));

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("Access denied to chat participants");
        }

        var participants = await _dbContext.ChatMemberships
            .Where(cml => cml.ChatId == chatId)
            .Select(cml => new ChatParticipantDto
            {
                ChatId = cml.ChatId,
                MemberId = cml.Member.UserId,
                MemberName = cml.Member.Name,
                MemberPhotoUrl = cml.Member.PhotoUrl,
                RoleInChat = cml.RoleInChat,
                JoinedAt = cml.JoinedAt
            })
            .ToListAsync();

        return participants;
    }

    public async Task<ChatParticipantDto> AddParticipantAsync(string requestingUserId, long chatId, AddParticipantDto addParticipantDto)
    {
        // Check if requesting member is owner of the chat
        var userMembership = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == requestingUserId);

        if (userMembership == null)
        {
            throw new KeyNotFoundException("Chat not found or user is not a member");
        }

        if (userMembership.RoleInChat != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only chat owners can add participants");
        }

        // Check if member to be added exists
        var memberToAdd = await _dbContext.Members.FirstOrDefaultAsync(m => m.UserId == addParticipantDto.UserId);
        if (memberToAdd == null)
        {
            throw new ArgumentException("Member to add not found", nameof(addParticipantDto.UserId));
        }

        // Check if member is already in the chat
        var existingLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == addParticipantDto.UserId);

        if (existingLink != null)
        {
            throw new InvalidOperationException("Member is already a participant in this chat");
        }

        // Check if it's a personal chat (can't add more participants)
        var chat = await _dbContext.Chat.FindAsync(chatId);
        if (chat != null && chat.Type == ChatType.Personal)
        {
            throw new InvalidOperationException("Cannot add participants to personal chats");
        }

        var newParticipantLink = new ChatMembership
        {
            ChatId = chatId,
            MemberId = memberToAdd.Id,
            RoleInChat = addParticipantDto.RoleInChat,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.ChatMemberships.Add(newParticipantLink);
        await _dbContext.SaveChangesAsync();

        return new ChatParticipantDto
        {
            ChatId = chatId,
            MemberId = addParticipantDto.UserId,
            MemberName = memberToAdd.Name,
            MemberPhotoUrl = memberToAdd.PhotoUrl,
            RoleInChat = addParticipantDto.RoleInChat,
            JoinedAt = newParticipantLink.JoinedAt
        };
    }

    public async Task UpdateParticipantRoleAsync(string requestingUserId, long chatId, string participantUserId, UpdateParticipantRoleDto updateRoleDto)
    {
        // Check if requesting member is owner of the chat
        var requestingMemberLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == requestingUserId);

        if (requestingMemberLink == null)
        {
            throw new KeyNotFoundException("Chat not found or user is not a member");
        }

        if (requestingMemberLink.RoleInChat != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only chat owners can update participant roles");
        }

        // Find the participant to update
        var participantLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == participantUserId);

        if (participantLink == null)
        {
            throw new KeyNotFoundException("Participant not found in this chat");
        }

        // Prevent owner from changing their own role
        if (participantUserId == requestingUserId)
        {
            throw new InvalidOperationException("Cannot change your own role");
        }

        participantLink.RoleInChat = updateRoleDto.RoleInChat;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveParticipantAsync(string requestingUserId, long chatId, string participantUserId)
    {
        // Check if requesting member is owner of the chat
        var requestingMemberLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == requestingUserId);

        if (requestingMemberLink == null)
        {
            throw new KeyNotFoundException("Chat not found or user is not a member");
        }

        if (requestingMemberLink.RoleInChat != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only chat owners can remove participants");
        }

        // Find the participant to remove
        var participantLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == participantUserId);

        if (participantLink == null)
        {
            throw new KeyNotFoundException("Participant not found in this chat");
        }

        // Prevent owner from removing themselves
        if (participantUserId == requestingUserId)
        {
            throw new InvalidOperationException("Cannot remove yourself from the chat. Use leave chat instead.");
        }

        _dbContext.ChatMemberships.Remove(participantLink);
        await _dbContext.SaveChangesAsync();
    }

    public async Task JoinChatAsync(string requestingUserId, long chatId)
    {
        // Check if member exists
        var member = await _dbContext.Members.FirstOrDefaultAsync(m => m.UserId == requestingUserId);
        if (member == null)
        {
            throw new ArgumentException("Member not found", nameof(requestingUserId));
        }

        // Check if chat exists and is public
        var chat = await _dbContext.Chat.FindAsync(chatId);
        if (chat == null)
        {
            throw new KeyNotFoundException("Chat not found");
        }

        if (chat.Type != ChatType.Public)
        {
            throw new InvalidOperationException("Can only join public chats");
        }

        // Check if member is already in the chat
        var existingLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == requestingUserId);

        if (existingLink != null)
        {
            throw new InvalidOperationException("Member is already a participant in this chat");
        }

        var newParticipantLink = new ChatMembership
        {
            ChatId = chatId,
            MemberId = member.Id,
            RoleInChat = ChatRole.Member,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.ChatMemberships.Add(newParticipantLink);
        await _dbContext.SaveChangesAsync();
    }

    public async Task LeaveChatAsync(string requestingUserId, long chatId)
    {
        // Find the participant link
        var participantLink = await _dbContext.ChatMemberships
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.Member.UserId == requestingUserId);

        if (participantLink == null)
        {
            throw new KeyNotFoundException("Member is not a participant in this chat");
        }

        // Check if this is the owner and if there are other participants
        if (participantLink.RoleInChat == ChatRole.Owner)
        {
            var otherParticipants = await _dbContext.ChatMemberships
                .Where(cml => cml.ChatId == chatId && cml.Member.UserId != requestingUserId)
                .ToListAsync();

            if (otherParticipants.Any())
            {
                throw new InvalidOperationException("Cannot leave chat as owner while other participants exist. Transfer ownership or remove all participants first.");
            }
        }

        _dbContext.ChatMemberships.Remove(participantLink);
        await _dbContext.SaveChangesAsync();
    }
}