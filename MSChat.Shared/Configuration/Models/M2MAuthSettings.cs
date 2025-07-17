using System.ComponentModel.DataAnnotations;

namespace MSChat.Shared.Configuration.Models;

public class M2MAuthSettings
{
    public const string Position = "M2MAuth";

    [Required]
    [Url]
    public string Authority { get; init; } = "";

    [Required]
    public string ClientId { get; init; } = "";

    [Required]
    public string ClientSecret{ get; init; } = "";
}
