using System.ComponentModel.DataAnnotations;

namespace MSChat.Chat.Models.DTOs;

public class AddParticipantDto
{
    [Required]
    public long MemberId { get; set; }
    
    [EnumDataType(typeof(ChatRole))]
    public ChatRole RoleInChat { get; set; } = ChatRole.Member;
}