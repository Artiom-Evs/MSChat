using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public interface IChatsService
{
    Task<IEnumerable<ChatDto>> GetChatsAsync(string userId, string? search = null);
    Task<ChatDto?> GetChatByIdAsync(string userId, long chatId);
    Task<ChatDto> CreateChatAsync(string userId, CreateChatDto createChatDto);
    Task UpdateChatAsync(string userId, long chatId, UpdateChatDto updateChatDto);
    Task DeleteChatAsync(string userId, long chatId);
}
