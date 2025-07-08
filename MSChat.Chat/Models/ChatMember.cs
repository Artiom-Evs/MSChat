namespace MSChat.Chat.Models;

public class ChatMember
{
    public long Id { get; set; }
    public string UserId { get; init; } = "";
    public string Name { get; set; } = "";
    public string? PhotoUrl { get; set; }
    public ICollection<ChatMembership> Chats { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}