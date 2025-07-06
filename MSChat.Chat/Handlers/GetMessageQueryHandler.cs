using MediatR;
using MSChat.Chat.Models.DTOs;
using MSChat.Chat.Queries;
using MSChat.Chat.Services;

namespace MSChat.Chat.Handlers;

public class GetMessageQueryHandler : IRequestHandler<GetMessageQuery, MessageDto?>
{
    private readonly IMessagesService _messagesService;

    public GetMessageQueryHandler(IMessagesService messagesService)
    {
        _messagesService = messagesService;
    }

    public async Task<MessageDto?> Handle(GetMessageQuery request, CancellationToken cancellationToken)
    {
        return await _messagesService.GetMessageByIdAsync(request.MemberId, request.MessageId);
    }
}
