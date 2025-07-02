using Microsoft.EntityFrameworkCore;

namespace MSChat.Chat.Models;

[PrimaryKey(nameof(ChatMemberLink.ChatId), nameof(ChatMemberLink.MemberId))]
public class ChatMemberLink
{
    public required long ChatId { get; set; }
    public required long MemberId { get; set; }
    public required ChatRole RoleInChat { get; set; }
    public required DateTime JoinedAt { get; set; }
}

public enum ChatRole
{
    Owner = 1,
    Member
}