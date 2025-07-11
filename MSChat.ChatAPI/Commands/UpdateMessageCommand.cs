using MediatR;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Commands;

public record UpdateMessageCommand(long MemberId, long ChatId, long MessageIdInChat, UpdateMessageDto UpdateMessage) : IRequest;
