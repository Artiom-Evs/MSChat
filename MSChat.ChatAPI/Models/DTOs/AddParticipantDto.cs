using System.ComponentModel.DataAnnotations;

namespace MSChat.ChatAPI.Models.DTOs;

public class AddParticipantDto
{
    [Required]
    public string UserId { get; set; } = "";
    
    [EnumDataType(typeof(ChatRole))]
    public ChatRole RoleInChat { get; set; } = ChatRole.Member;
}