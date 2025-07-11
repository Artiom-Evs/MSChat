using Microsoft.EntityFrameworkCore;

namespace MSChat.ChatAPI.Models;

public class ChatMembership
{
    public long ChatId { get; set; }
    public Chat Chat { get; set; } = null!;
    public long MemberId { get; set; }
    public ChatMember Member { get; set; } = null!;

    public ChatRole RoleInChat { get; set; }
    public long LastReadedMessageId { get; set; }
    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;
}

public enum ChatRole
{
    Owner = 1,
    Member
}