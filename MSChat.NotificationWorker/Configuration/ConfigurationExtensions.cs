using MSChat.Shared.Configuration.Models;
using System.ComponentModel.DataAnnotations;

namespace MSChat.NotificationWorker.Configuration;

public static class ConfigurationExtensions
{
    public static ServicesSettings GetServicesSettings(this IConfiguration configuration)
    {
        var servicesSettings = configuration
            .GetRequiredSection(ServicesSettings.Position)
            .Get<ServicesSettings>()!;
        var servicesSettingsValidationContext = new ValidationContext(servicesSettings);
        Validator.ValidateObject(servicesSettings!, servicesSettingsValidationContext, validateAllProperties: true);

        return servicesSettings;
    }
}
