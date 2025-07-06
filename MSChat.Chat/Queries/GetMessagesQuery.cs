using MediatR;
using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Queries;

public record GetMessagesQuery(long MemberId, long ChatId, int Page = 1, int PageSize = 50) : IRequest<IEnumerable<MessageDto>>;