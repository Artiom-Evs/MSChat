namespace MSChat.Chat.Models;

public class ChatMember
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public string? PhotoUrl { get; set; }
    public required ICollection<ChatMemberLink> Chats { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}