using Microsoft.EntityFrameworkCore;
using MSChat.Chat.Data;
using MSChat.Chat.Models;
using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Services;

public class ChatParticipantsService : IChatParticipantsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ChatParticipantsService> _logger;

    public ChatParticipantsService(ApplicationDbContext dbContext, ILogger<ChatParticipantsService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<ChatParticipantDto>> GetChatParticipantsAsync(long requestingMemberId, long chatId)
    {
        // Check if requesting member has access to the chat
        var hasAccess = await _dbContext.ChatMemberLinks
            .AnyAsync(cml => cml.ChatId == chatId && (cml.Chat.Type == ChatType.Public || cml.MemberId == requestingMemberId));

        if (!hasAccess)
        {
            throw new UnauthorizedAccessException("Access denied to chat participants");
        }

        var participants = await _dbContext.ChatMemberLinks
            .Where(cml => cml.ChatId == chatId)
            .Select(cml => new ChatParticipantDto
            {
                ChatId = cml.ChatId,
                MemberId = cml.MemberId,
                MemberName = cml.Member.Name,
                MemberPhotoUrl = cml.Member.PhotoUrl,
                RoleInChat = cml.RoleInChat,
                JoinedAt = cml.JoinedAt
            })
            .ToListAsync();

        return participants;
    }

    public async Task<ChatParticipantDto> AddParticipantAsync(long requestingMemberId, long chatId, AddParticipantDto addParticipantDto)
    {
        // Check if requesting member is owner of the chat
        var requestingMemberLink = await _dbContext.ChatMemberLinks
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == requestingMemberId);

        if (requestingMemberLink == null)
        {
            throw new KeyNotFoundException("Chat not found or user is not a member");
        }

        if (requestingMemberLink.RoleInChat != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only chat owners can add participants");
        }

        // Check if member to be added exists
        var memberToAdd = await _dbContext.Members.FindAsync(addParticipantDto.MemberId);
        if (memberToAdd == null)
        {
            throw new ArgumentException("Member to add not found", nameof(addParticipantDto.MemberId));
        }

        // Check if member is already in the chat
        var existingLink = await _dbContext.ChatMemberLinks
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == addParticipantDto.MemberId);

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

        var newParticipantLink = new ChatMemberLink
        {
            ChatId = chatId,
            MemberId = addParticipantDto.MemberId,
            RoleInChat = addParticipantDto.RoleInChat,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.ChatMemberLinks.Add(newParticipantLink);
        await _dbContext.SaveChangesAsync();

        return new ChatParticipantDto
        {
            ChatId = chatId,
            MemberId = addParticipantDto.MemberId,
            MemberName = memberToAdd.Name,
            MemberPhotoUrl = memberToAdd.PhotoUrl,
            RoleInChat = addParticipantDto.RoleInChat,
            JoinedAt = newParticipantLink.JoinedAt
        };
    }

    public async Task UpdateParticipantRoleAsync(long requestingMemberId, long chatId, long participantMemberId, UpdateParticipantRoleDto updateRoleDto)
    {
        // Check if requesting member is owner of the chat
        var requestingMemberLink = await _dbContext.ChatMemberLinks
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == requestingMemberId);

        if (requestingMemberLink == null)
        {
            throw new KeyNotFoundException("Chat not found or user is not a member");
        }

        if (requestingMemberLink.RoleInChat != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only chat owners can update participant roles");
        }

        // Find the participant to update
        var participantLink = await _dbContext.ChatMemberLinks
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == participantMemberId);

        if (participantLink == null)
        {
            throw new KeyNotFoundException("Participant not found in this chat");
        }

        // Prevent owner from changing their own role
        if (participantMemberId == requestingMemberId)
        {
            throw new InvalidOperationException("Cannot change your own role");
        }

        participantLink.RoleInChat = updateRoleDto.RoleInChat;
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveParticipantAsync(long requestingMemberId, long chatId, long participantMemberId)
    {
        // Check if requesting member is owner of the chat
        var requestingMemberLink = await _dbContext.ChatMemberLinks
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == requestingMemberId);

        if (requestingMemberLink == null)
        {
            throw new KeyNotFoundException("Chat not found or user is not a member");
        }

        if (requestingMemberLink.RoleInChat != ChatRole.Owner)
        {
            throw new UnauthorizedAccessException("Only chat owners can remove participants");
        }

        // Find the participant to remove
        var participantLink = await _dbContext.ChatMemberLinks
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == participantMemberId);

        if (participantLink == null)
        {
            throw new KeyNotFoundException("Participant not found in this chat");
        }

        // Prevent owner from removing themselves
        if (participantMemberId == requestingMemberId)
        {
            throw new InvalidOperationException("Cannot remove yourself from the chat. Use leave chat instead.");
        }

        _dbContext.ChatMemberLinks.Remove(participantLink);
        await _dbContext.SaveChangesAsync();
    }

    public async Task JoinChatAsync(long memberId, long chatId)
    {
        // Check if member exists
        var member = await _dbContext.Members.FindAsync(memberId);
        if (member == null)
        {
            throw new ArgumentException("Member not found", nameof(memberId));
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
        var existingLink = await _dbContext.ChatMemberLinks
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == memberId);

        if (existingLink != null)
        {
            throw new InvalidOperationException("Member is already a participant in this chat");
        }

        var newParticipantLink = new ChatMemberLink
        {
            ChatId = chatId,
            MemberId = memberId,
            RoleInChat = ChatRole.Member,
            JoinedAt = DateTime.UtcNow
        };

        _dbContext.ChatMemberLinks.Add(newParticipantLink);
        await _dbContext.SaveChangesAsync();
    }

    public async Task LeaveChatAsync(long memberId, long chatId)
    {
        // Find the participant link
        var participantLink = await _dbContext.ChatMemberLinks
            .FirstOrDefaultAsync(cml => cml.ChatId == chatId && cml.MemberId == memberId);

        if (participantLink == null)
        {
            throw new KeyNotFoundException("Member is not a participant in this chat");
        }

        // Check if this is the owner and if there are other participants
        if (participantLink.RoleInChat == ChatRole.Owner)
        {
            var otherParticipants = await _dbContext.ChatMemberLinks
                .Where(cml => cml.ChatId == chatId && cml.MemberId != memberId)
                .ToListAsync();

            if (otherParticipants.Any())
            {
                throw new InvalidOperationException("Cannot leave chat as owner while other participants exist. Transfer ownership or remove all participants first.");
            }
        }

        _dbContext.ChatMemberLinks.Remove(participantLink);
        await _dbContext.SaveChangesAsync();
    }
}