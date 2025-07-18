using System.ComponentModel.DataAnnotations;

namespace MSChat.NotificationWorker.Configuration;

public class EmailSettings
{
    public const string Position = "Email";

    [Required]
    public string SmtpServer { get; init; } = "";
    [Required]
    public int SmtpPort { get; init; }
    [Required]
    public string Username { get; init; } = "";
    [Required]
    public string Password { get; init; } = "";
    [Required]
    public string FromEmail { get; init; } = "";
    public string? FromName { get; init; } = null;
}
