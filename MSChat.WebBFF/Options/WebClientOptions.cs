using System.ComponentModel.DataAnnotations;

namespace MSChat.WebBFF.Options;

public class WebClientOptions
{
    public const string Position = "WebClient";

    [Required]
    public string OidcServerUri { get; init; } = "";
    [Required]
    public string OidcClientId { get; init; } = "";
    [Required]
    public string ChatApiUri { get; init; } = "";
    [Required]
    public string PresenceApiUri { get; init; } = "";
}
