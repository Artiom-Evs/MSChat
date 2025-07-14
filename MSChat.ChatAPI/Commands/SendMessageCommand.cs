using MediatR;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Commands;

public record SendMessageCommand(string UserId, long ChatId, CreateMessageDto NewMessage) : IRequest<MessageDto>;
