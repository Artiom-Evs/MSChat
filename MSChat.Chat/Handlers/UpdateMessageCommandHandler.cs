using MediatR;
using Microsoft.AspNetCore.SignalR;
using MSChat.Chat.Commands;
using MSChat.Chat.Hubs;
using MSChat.Chat.Services;

namespace MSChat.Chat.Handlers;

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
        await _messagesService.UpdateMessageAsync(request.MemberId, request.MessageId, request.UpdateMessage);
        var updatedMessage = await _messagesService.GetMessageByIdAsync(request.MemberId, request.MessageId);
        var groupName = ChatHub.GetChatGroupName(request.ChatId);
        await _chatHub.Clients.Group(groupName).MessageUpdated(updatedMessage!);
    }
}