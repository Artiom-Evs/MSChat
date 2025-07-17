using System.ComponentModel.DataAnnotations;

namespace MSChat.NotificationWorker.Configuration;

public class ServicesSettings
{
    public const string Position = "Services";

    [Required]
    [Url]
    public string ChatApiUri { get; init; } = "";

    [Required]
    [Url]
    public string PresenceApiUri { get; init; } = "";

    [Required]
    [Url]
    public string AuthApiUri { get; init; } = "";
}
