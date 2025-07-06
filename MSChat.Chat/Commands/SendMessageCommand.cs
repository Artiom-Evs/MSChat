using MediatR;
using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Commands;

public record SendMessageCommand(long MemberId, long ChatId, CreateMessageDto NewMessage) : IRequest<MessageDto>;
