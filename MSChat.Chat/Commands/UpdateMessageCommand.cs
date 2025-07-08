using MediatR;
using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Commands;

public record UpdateMessageCommand(long MemberId, long ChatId, long MessageIdInChat, UpdateMessageDto UpdateMessage) : IRequest;
