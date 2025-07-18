namespace MSChat.ChatAPI.Models.DTOs;

public class ChatMembershipDto
{
    public string MemberId { get; init; } = "";
    public long ChatId { get; init; }
    public string ChatName { get; init; } = "";
    public long LastReadMessageId { get; init; }
}
