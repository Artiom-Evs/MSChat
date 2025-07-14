using MediatR;
using Microsoft.AspNetCore.SignalR;
using MSChat.ChatAPI.Commands;
using MSChat.ChatAPI.Hubs;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Handlers;

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand>
{
    private readonly IMessagesService _messagesService;
    private readonly IHubContext<ChatHub, IChatHubClient> _chatHub;

    public DeleteMessageCommandHandler(IMessagesService messagesService, IHubContext<ChatHub, IChatHubClient> chatHub)
    {
        _messagesService = messagesService;
        _chatHub = chatHub;
    }

    public async Task Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        await _messagesService.DeleteMessageAsync(request.UserId, request.ChatId, request.MessageIdInChat);
        var groupName = ChatHub.GetChatGroupName(request.ChatId);
        await _chatHub.Clients.Group(groupName).MessageDeleted(request.MessageIdInChat);
    }
}