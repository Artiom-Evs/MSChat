namespace MSChat.ChatAPI.Models.DTOs;

public class ChatParticipantDto
{
    public long ChatId { get; set; }
    public long MemberId { get; set; }
    public string MemberName { get; set; } = "";
    public string? MemberPhotoUrl { get; set; }
    public ChatRole RoleInChat { get; set; }
    public DateTime JoinedAt { get; set; }
}