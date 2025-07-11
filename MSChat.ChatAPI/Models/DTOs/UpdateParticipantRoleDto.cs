using System.ComponentModel.DataAnnotations;

namespace MSChat.ChatAPI.Models.DTOs;

public class UpdateParticipantRoleDto
{
    [Required]
    [EnumDataType(typeof(ChatRole))]
    public ChatRole RoleInChat { get; set; }
}