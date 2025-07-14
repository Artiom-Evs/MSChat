using MediatR;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Queries;

public record GetMessageQuery(string userId, long ChatId, long MessageIdInChat): IRequest<MessageDto?>;
