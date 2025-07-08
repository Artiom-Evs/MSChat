using MediatR;
using Microsoft.AspNetCore.SignalR;
using MSChat.Chat.Commands;
using MSChat.Chat.Hubs;
using MSChat.Chat.Services;

namespace MSChat.Chat.Handlers;

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
        await _messagesService.DeleteMessageAsync(request.MemberId, request.ChatId, request.MessageIdInChat);
        var groupName = ChatHub.GetChatGroupName(request.ChatId);
        await _chatHub.Clients.Group(groupName).MessageDeleted(request.MessageIdInChat);
    }
}