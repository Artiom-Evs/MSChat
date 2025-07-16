using MediatR;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Queries;

public record GetChatMembershipsQuery(long ChatId): IRequest<IEnumerable<ChatMembershipDto>>;
