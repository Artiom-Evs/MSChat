using MediatR;
using MSChat.Chat.Models.DTOs;

namespace MSChat.Chat.Queries;

public record GetMessageQuery(long MemberId, long ChatId, long MessageId): IRequest<MessageDto?>;
