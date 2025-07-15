using System.ComponentModel.DataAnnotations;

namespace MSChat.Shared.Configuration.Models;

public class RabbitMQSettings
{
    public static string Position = "RabbitMQ";

    [Required]
    public string Host { get; set; } = "";
    [Required]
    public string Username { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";
}
