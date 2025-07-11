using MediatR;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Queries;

public record GetMessageQuery(long MemberId, long ChatId, long MessageIdInChat): IRequest<MessageDto?>;
