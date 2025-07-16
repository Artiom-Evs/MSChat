using System.ComponentModel.DataAnnotations;

namespace MSChat.Shared.Configuration.Models;

public class RedisSettings
{
    public const string Position = "Redis";

    [Required]
    public string ConnectionString { get; init; } = "";
}
