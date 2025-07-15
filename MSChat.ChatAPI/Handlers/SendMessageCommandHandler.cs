using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using MSChat.ChatAPI.Commands;
using MSChat.ChatAPI.Hubs;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Services;
using MSChat.Shared.Events;

namespace MSChat.ChatAPI.Handlers;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessagesService _messagesService;
    private readonly IHubContext<ChatHub, IChatHubClient> _chatHub;
    private readonly IPublishEndpoint _publish;

    public SendMessageCommandHandler(IMessagesService messagesService, IHubContext<ChatHub, IChatHubClient> chatHub, IPublishEndpoint publish)
    {
        _messagesService = messagesService;
        _chatHub = chatHub;
        _publish = publish;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var createdMessage = await _messagesService.CreateMessageAsync(request.UserId, request.ChatId, request.NewMessage);
        var groupName = ChatHub.GetChatGroupName(createdMessage.ChatId);

        await _chatHub.Clients.Group(groupName).NewMessageSent(createdMessage);
        await _publish.Publish(new ChatMessageSent(createdMessage.SenderId, createdMessage.ChatId, createdMessage.Id, createdMessage.Text, createdMessage.CreatedAt));

        return createdMessage;
    }
}
