using MediatR;
using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Queries;

public record GetMessagesQuery(long MemberId, long ChatId, int Limit, long? Offset) : IRequest<IEnumerable<MessageDto>>;