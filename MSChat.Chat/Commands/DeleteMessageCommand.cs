using MediatR;

namespace MSChat.Chat.Commands;

public record DeleteMessageCommand(long MemberId, long ChatId, long MessageId) : IRequest;