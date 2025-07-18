using MassTransit;
using MSChat.NotificationWorker.Configuration;
using MSChat.NotificationWorker.Services;
using MSChat.Shared.Configuration.Extensions;
using MSChat.Shared.Configuration.Models;
using MSChat.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<M2MAuthSettings>()
    .BindConfiguration(M2MAuthSettings.Position)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<ServicesSettings>()
    .BindConfiguration(ServicesSettings.Position)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<EmailSettings>()
    .BindConfiguration(EmailSettings.Position)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var rmqSettings = builder.Configuration.GetRabbitMQSettings();
var servicesSettings = builder.Configuration.GetServicesSettings();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rmqSettings.Host, "/", h =>
        {
            h.Username(rmqSettings.Username);
            h.Password(rmqSettings.Password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHttpClient();
builder.Services
    .AddGrpcClient<ChatAPI.ChatAPIClient>(options =>
    {
        options.Address = new Uri(servicesSettings.ChatApiUri);
    })
    .ConfigureChannel(options =>
    {
        options.UnsafeUseInsecureChannelCallCredentials = builder.Environment.IsDevelopment();
    })
    .AddCallCredentials(async (c, m, p) =>
    {
        var tokenProvider = p.GetRequiredService<IAccessTokenProvider>();
        var accessToken = await tokenProvider.GetAccessTokenAsync();
        m.Add("Authorization", $"Bearer {accessToken}");
    });
builder.Services
    .AddGrpcClient<PresenceAPI.PresenceAPIClient>(options =>
    {
        options.Address = new Uri(servicesSettings.PresenceApiUri);
    })
    .ConfigureChannel(options =>
    {
        options.UnsafeUseInsecureChannelCallCredentials = builder.Environment.IsDevelopment();
    })
    .AddCallCredentials(async (c, m, p) =>
    {
        var tokenProvider = p.GetRequiredService<IAccessTokenProvider>();
        var accessToken = await tokenProvider.GetAccessTokenAsync();
        m.Add("Authorization", $"Bearer {accessToken}");
    });

builder.Services.AddSingleton<IAccessTokenProvider, AccessTokenProvider>();
builder.Services.AddSingleton<IAuthAPIClient, AuthAPIClient>();
builder.Services.AddSingleton<INotificationService, NotificationService>();
builder.Services.AddSingleton<IEmailService, EmailService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
