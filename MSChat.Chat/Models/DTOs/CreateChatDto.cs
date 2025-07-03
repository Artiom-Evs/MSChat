using System.ComponentModel.DataAnnotations;

namespace MSChat.Chat.Models.DTOs;

public class CreateChatDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = "";
    
    [Required]
    [EnumDataType(typeof(ChatType))]
    public ChatType Type { get; set; }
    
    public long? OtherMemberId { get; set; }
}