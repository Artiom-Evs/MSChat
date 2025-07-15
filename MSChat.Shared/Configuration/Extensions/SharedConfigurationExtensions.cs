
using Microsoft.Extensions.Configuration;
using MSChat.ChatAPI.Configurations;
using MSChat.Shared.Configuration.Models;
using System.ComponentModel.DataAnnotations;

namespace MSChat.Shared.Configuration.Extensions;

public static class SharedConfigurationExtensions
{
    /// <summary>
    /// returns validated API authentication settings
    /// </summary>
    public static ApiAuthSettings GetAuthSettings(this IConfiguration configuration)
    {
        var authSettings = configuration
            .GetRequiredSection(ApiAuthSettings.Position)
            .Get<ApiAuthSettings>()!;
        var authSettingsValidationContext = new ValidationContext(authSettings);
        Validator.ValidateObject(authSettings!, authSettingsValidationContext, validateAllProperties: true);

        return authSettings;
    }

    /// <summary>
    /// returns validated API CORS settings
    /// </summary>
    public static ApiCorsSettings GetCorsSettings(this IConfiguration configuration)
    {
        var corsSettings = configuration
            .GetRequiredSection(ApiCorsSettings.Position)
            .Get<ApiCorsSettings>()!;
        var corsSettingsValidationContext = new ValidationContext(corsSettings);
        Validator.ValidateObject(corsSettings!, corsSettingsValidationContext, validateAllProperties: true);

        return corsSettings;
    }

    /// <summary>
    /// returns validated Redis settings
    /// </summary>
    public static RedisSettings GetRedisSettings(this IConfiguration configuration)
    {
        var redisSettings = configuration
            .GetRequiredSection(RedisSettings.Position)
            .Get<RedisSettings>()!;
        var redisSettingsValidationContext = new ValidationContext(redisSettings);
        Validator.ValidateObject(redisSettings!, redisSettingsValidationContext, validateAllProperties: true);

        return redisSettings;
    }

    public static RabbitMQSettings GetRabbitMQSettings(this IConfiguration configuration)
    {
        var rmqSettings = configuration
            .GetRequiredSection(RabbitMQSettings.Position)
            .Get<RabbitMQSettings>()!;
        var rmqSettingsValidationContext = new ValidationContext(rmqSettings);
        Validator.ValidateObject(rmqSettings!, rmqSettingsValidationContext, validateAllProperties: true);

        return rmqSettings;
    }
}
