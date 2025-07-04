using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Services;

public interface IMessagesService
{
    Task<IEnumerable<MessageDto>> GetMessagesAsync(long memberId, long chatId, int page = 1, int pageSize = 50);
    Task<MessageDto?> GetMessageByIdAsync(long memberId, long messageId);
    Task<MessageDto> CreateMessageAsync(long memberId, long chatId, CreateMessageDto createMessageDto);
    Task UpdateMessageAsync(long memberId, long messageId, UpdateMessageDto updateMessageDto);
    Task DeleteMessageAsync(long memberId, long messageId);
}