using MediatR;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Queries;

public record GetMessagesQuery(long MemberId, long ChatId, int Limit, long? Offset) : IRequest<IEnumerable<MessageDto>>;