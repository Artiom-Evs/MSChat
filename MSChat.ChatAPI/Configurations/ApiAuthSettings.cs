using System.ComponentModel.DataAnnotations;

namespace MSChat.ChatAPI.Configurations;

public class ApiAuthSettings
{
    public const string Position = "ApiAuth";

    [Required]
    [Url]
    public string Authority { get; set; } = string.Empty;
    [Required]
    public string ValidIssuer { get; set; } = string.Empty;
}
