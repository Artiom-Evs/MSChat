using MediatR;
using Microsoft.AspNetCore.SignalR;
using MSChat.ChatAPI.Commands;
using MSChat.ChatAPI.Hubs;
using MSChat.ChatAPI.Models.DTOs;
using MSChat.ChatAPI.Services;

namespace MSChat.ChatAPI.Handlers;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessagesService _messagesService;
    private readonly IHubContext<ChatHub, IChatHubClient> _chatHub;

    public SendMessageCommandHandler(IMessagesService messagesService, IHubContext<ChatHub, IChatHubClient> chatHub)
    {
        _messagesService = messagesService;
        _chatHub = chatHub;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var createdMessage = await _messagesService.CreateMessageAsync(request.MemberId, request.ChatId, request.NewMessage);
        var groupName = ChatHub.GetChatGroupName(createdMessage.ChatId);
        await _chatHub.Clients.Group(groupName).NewMessageSent(createdMessage);
        return createdMessage;
    }
}
