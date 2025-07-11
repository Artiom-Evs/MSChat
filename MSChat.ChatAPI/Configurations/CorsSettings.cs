using System.ComponentModel.DataAnnotations;

namespace MSChat.ChatAPI.Configurations;

public class CorsSettings
{
    public const string Position = "Cors";

    [Required]
    public string AllowedOrigins { get; init; } = "";
}
