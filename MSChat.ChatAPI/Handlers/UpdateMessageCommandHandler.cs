using MediatR;
using Microsoft.AspNetCore.SignalR;
using MSChat.ChatAPI.Commands;
using MSChat.ChatAPI.Hubs;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Handlers;

public class UpdateMessageCommandHandler : IRequestHandler<UpdateMessageCommand>
{
    private readonly IMessagesService _messagesService;
    private readonly IHubContext<ChatHub, IChatHubClient> _chatHub;

    public UpdateMessageCommandHandler(IMessagesService messagesService, IHubContext<ChatHub, IChatHubClient> chatHub)
    {
        _messagesService = messagesService;
        _chatHub = chatHub;
    }

    public async Task Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        await _messagesService.UpdateMessageAsync(request.UserId, request.ChatId, request.MessageIdInChat, request.UpdateMessage);
        var updatedMessage = await _messagesService.GetMessageByIdAsync(request.UserId, request.ChatId, request.MessageIdInChat);
        var groupName = ChatHub.GetChatGroupName(request.ChatId);
        await _chatHub.Clients.Group(groupName).MessageUpdated(updatedMessage!);
    }
}