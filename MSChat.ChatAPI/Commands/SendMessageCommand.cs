using MediatR;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Commands;

public record SendMessageCommand(long MemberId, long ChatId, CreateMessageDto NewMessage) : IRequest<MessageDto>;
