using MediatR;
using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Queries;

namespace MSChat.ChatAPI.Handlers;

public class GetChatMembershipsQueryHandler : IRequestHandler<GetChatMembershipsQuery, IEnumerable<ChatMembershipDto>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetChatMembershipsQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<ChatMembershipDto>> Handle(GetChatMembershipsQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext.ChatMemberships
            .Where(cm => cm.ChatId == request.ChatId)
            .Select(cm => new ChatMembershipDto()
            {
                ChatId = cm.ChatId,
                ChatName = cm.Chat.Name,
                MemberId = cm.Member.UserId,
                LastReadMessageId = cm.LastReadedMessageId,
            })
            .ToListAsync();
    }
}
