using MediatR;
using MSChat.Chat.Models.DTOs;
using MSChat.Chat.Queries;
using MSChat.Chat.Services;

namespace MSChat.Chat.Handlers;

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