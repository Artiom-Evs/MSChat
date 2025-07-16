using System.ComponentModel.DataAnnotations;

namespace MSChat.Shared.Configuration.Models;

public class ApiCorsSettings
{
    public const string Position = "Cors";

    [Required]
    public string AllowedOrigins { get; init; } = "";
}
