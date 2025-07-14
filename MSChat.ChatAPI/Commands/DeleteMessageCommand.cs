using MediatR;

namespace MSChat.ChatAPI.Commands;

public record DeleteMessageCommand(string UserId, long ChatId, long MessageIdInChat) : IRequest;