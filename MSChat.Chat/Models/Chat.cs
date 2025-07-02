namespace MSChat.Chat.Models;

public class Chat
{
    public required long Id { get; set; }
    public required string Name { get; set; }
    public required ChatType Type { get; set; }
    public required ICollection<ChatMemberLink> Members { get; set; }
    public required ICollection<Message> Messages { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public enum ChatType
{
    Public = 1,
    Personal
}