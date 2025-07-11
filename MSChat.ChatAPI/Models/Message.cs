namespace MSChat.ChatAPI.Models;

public class Message
{
    public long Id { get; set; }
    public long IdInChat { get; set; }
    public long ChatId { get; set; }
    public long SenderId { get; set; }
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}