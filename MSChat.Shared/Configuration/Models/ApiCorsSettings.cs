using System.ComponentModel.DataAnnotations;

namespace MSChat.ChatAPI.Configurations;

public class ApiCorsSettings
{
    public const string Position = "Cors";

    [Required]
    public string AllowedOrigins { get; init; } = "";
}
