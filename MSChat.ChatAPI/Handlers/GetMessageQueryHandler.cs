using MediatR;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Queries;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Handlers;

public class GetMessageQueryHandler : IRequestHandler<GetMessageQuery, MessageDto?>
{
    private readonly IMessagesService _messagesService;

    public GetMessageQueryHandler(IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    public async Task<MessageDto?> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
        return await _messagesService.GetMessageByIdAsync(request.userId, request.ChatId, request.MessageIdInChat);
    }
}
