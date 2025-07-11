using System.ComponentModel.DataAnnotations;

namespace MSChat.ChatAPI.Configurations;

public class RedisSettings
{
    public const string Position = "Redis";

    [Required]
    public string ConnectionString { get; init; } = "";
}
