using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Services;

public interface IChatParticipantsService
{
    Task<IEnumerable<ChatParticipantDto>> GetChatParticipantsAsync(long requestingMemberId, long chatId);
    Task<ChatParticipantDto> AddParticipantAsync(long requestingMemberId, long chatId, AddParticipantDto addParticipantDto);
    Task UpdateParticipantRoleAsync(long requestingMemberId, long chatId, long participantMemberId, UpdateParticipantRoleDto updateRoleDto);
    Task RemoveParticipantAsync(long requestingMemberId, long chatId, long participantMemberId);
    Task JoinChatAsync(long memberId, long chatId);
    Task LeaveChatAsync(long memberId, long chatId);
}