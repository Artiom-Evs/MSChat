using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public interface IChatsService
{
    Task<IEnumerable<ChatDto>> GetChatsAsync(long memberId, string? search = null);
    Task<ChatDto?> GetChatByIdAsync(long memberId, long chatId);
    Task<ChatDto> CreateChatAsync(long memberId, CreateChatDto createChatDto);
    Task UpdateChatAsync(long memberId, long chatId, UpdateChatDto updateChatDto);
    Task DeleteChatAsync(long memberId, long chatId);
}
