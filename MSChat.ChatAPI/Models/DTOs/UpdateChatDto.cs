using System.ComponentModel.DataAnnotations;

namespace MSChat.ChatAPI.Models.DTOs;

public class UpdateChatDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = "";
}