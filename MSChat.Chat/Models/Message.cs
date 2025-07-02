namespace MSChat.Chat.Models;

public class Message
{
    public required long Id { get; set; }
    public required long ChatId { get; set; }
    public required long SenderId { get; set; }
    public required string Text { get; set; }
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}