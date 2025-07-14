using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public interface IChatParticipantsService
{
    Task<IEnumerable<ChatParticipantDto>> GetChatParticipantsAsync(string requestingUserId, long chatId);
    Task<ChatParticipantDto> AddParticipantAsync(string requestingUserId, long chatId, AddParticipantDto addParticipantDto);
    Task UpdateParticipantRoleAsync(string requestingUserId, long chatId, string participantUserId, UpdateParticipantRoleDto updateRoleDto);
    Task RemoveParticipantAsync(string requestingUserId, long chatId, string participantUserId);
    Task JoinChatAsync(string requestingUserId, long chatId);
    Task LeaveChatAsync(string requestingUserId, long chatId);
}