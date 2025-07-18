using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using MSChat.ChatAPI.Queries;
using MSChat.Shared.Contracts;

namespace MSChat.ChatAPI.Services.Grpc;

[Authorize]
public class ChatAPIService : Shared.Contracts.ChatAPI.ChatAPIBase
{
    private readonly ISender _mediatr;

    public ChatAPIService(ISender mediatr)
    {
        _mediatr = mediatr;
    }

    public override async Task<GetChatMembershipsResponse> GetChatMemberships(GetChatMembershipsRequest request, ServerCallContext context)
    {
        var cmd = new GetChatMembershipsQuery(request.ChatId);
        var memberships = await _mediatr.Send(cmd);
        var responseMemberships = memberships.Select(m => new ChatMembership()
        {
            ChatId = m.ChatId,
            ChatName = m.ChatName,
            UserId = m.MemberId,
            LastReadMessageId = m.LastReadMessageId,
        });

        var response = new GetChatMembershipsResponse();
        response.Memberships.AddRange(responseMemberships);

        return response;
    }
}
