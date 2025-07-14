using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Services;

public interface IMessagesService
{
    Task<IEnumerable<MessageDto>> GetMessagesAsync(string userId, long chatId, int limit, long? offset);
    Task<MessageDto?> GetMessageByIdAsync(string userId, long chatId, long messageIdInChat);
    Task<MessageDto> CreateMessageAsync(string userId, long chatId, CreateMessageDto createMessageDto);
    Task UpdateMessageAsync(string userId, long chatId, long messageIdInChat, UpdateMessageDto updateMessageDto);
    Task DeleteMessageAsync(string userId, long chatId, long messageIdInChat);
}