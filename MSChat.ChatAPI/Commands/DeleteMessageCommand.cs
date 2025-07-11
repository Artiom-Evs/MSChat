using MediatR;

namespace MSChat.ChatAPI.Commands;

public record DeleteMessageCommand(long MemberId, long ChatId, long MessageIdInChat) : IRequest;