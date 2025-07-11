namespace MSChat.Chat.Models.DTOs;

public class ChatDto
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public ChatType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsInChat { get; set; }
    public int UnreadMessagesCount { get; set; }
    public ICollection<ChatParticipantDto>? Participants { get; set; }
    public MessageDto? LastMessage { get; set; }
}