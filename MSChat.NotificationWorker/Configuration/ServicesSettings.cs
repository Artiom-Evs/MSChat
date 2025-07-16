using System.ComponentModel.DataAnnotations;

namespace MSChat.NotificationWorker.Configuration;

public class ServicesSettings
{
    public const string Position = "Grpc";

    [Required]
    [Url]
    public string ChatApiUri { get; init; } = "";

    [Required]
    [Url]
    public string PresenceApiUri { get; init; } = "";
}
