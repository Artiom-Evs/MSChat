using System.ComponentModel.DataAnnotations;

namespace MSChat.ChatAPI.Models.DTOs;

public class UpdateMessageDto
{
    [Required]
    [StringLength(5000, MinimumLength = 1)]
    public string Text { get; set; } = "";
}