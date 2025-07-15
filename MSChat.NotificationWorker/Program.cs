using MassTransit;
using MSChat.Shared.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

var rmqSettings = builder.Configuration.GetRabbitMQSettings();

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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
