using MediatR;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Queries;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Handlers;

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
{
    private readonly IMessagesService _messagesService;

    public GetMessagesQueryHandler(IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        return await _messagesService.GetMessagesAsync(request.MemberId, request.ChatId, request.Limit, request.Offset);
    }
}