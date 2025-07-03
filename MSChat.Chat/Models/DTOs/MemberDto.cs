namespace MSChat.Chat.Models.DTOs;

public class MemberDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}