using System.ComponentModel.DataAnnotations;

namespace MSChat.Chat.Configurations;

public class ApiAuthSettings
{
    public const string Position = "ApiAuthSettings";

    [Required]
    [Url]
    public string Issuer { get; set; } = string.Empty;
}
