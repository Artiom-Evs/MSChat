namespace MSChat.ChatAPI.Models;

public class Chat
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public ChatType Type { get; set; }
    public ICollection<ChatMembership> Members { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

public enum ChatType
{
    Public = 1,
    Personal
}