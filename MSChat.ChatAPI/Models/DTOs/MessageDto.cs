namespace MSChat.ChatAPI.Models.DTOs;

public class MessageDto
{
    public long Id { get; set; }
    public long ChatId { get; set; }
    public string SenderId { get; set; } = "";
    public string SenderName { get; set; } = "";
    public string? SenderPhotoUrl { get; set; }
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted => DeletedAt.HasValue;
    public bool IsEdited => UpdatedAt.HasValue;
}