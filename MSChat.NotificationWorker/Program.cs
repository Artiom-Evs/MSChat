using MassTransit;
using MSChat.NotificationWorker.Configuration;
using MSChat.NotificationWorker.Services;
using MSChat.Shared.Configuration.Extensions;
using MSChat.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddGrpcClient<ChatAPI.ChatAPIClient>(options =>
{
    options.Address = new Uri(servicesSettings.ChatApiUri);
});
builder.Services.AddGrpcClient<PresenceAPI.PresenceAPIClient>(options =>
{
    options.Address = new Uri(servicesSettings.PresenceApiUri);
});

builder.Services.AddSingleton<INotificationService, NotificationService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
