using Microsoft.AspNetCore.SignalR;
using MSChat.ChatAPI.Models;
using MSChat.ChatAPI.Models.DTOs;

namespace MSChat.ChatAPI.Hubs;

public interface IChatHubClient
{
    Task NewMessageSent(MessageDto newMessage);
    Task MessageUpdated(MessageDto updatedMessage);
    Task MessageDeleted(long messageId);
}

public class ChatHub : Hub<IChatHubClient>
{
    public async Task SubscribeToChat(string chatId)
    {
        var groupName = GetChatGroupName(chatId);
        await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
    }

    public async Task UnsubscribeFromChat(string chatId)
    {
        var groupName = GetChatGroupName(chatId);
        await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
    }

    public static string GetChatGroupName(long chatId) => $"chat:{chatId}";
    public static string GetChatGroupName(string chatId) => $"chat:{chatId}";
}
