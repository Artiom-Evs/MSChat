using Microsoft.Extensions.Options;
using MSChat.WebBFF.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<WebClientOptions>()
    .BindConfiguration(WebClientOptions.Position)
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

app.UseHttpsRedirection();

app.MapGet("/app-config", (IOptions<WebClientOptions> options) =>
{
    return options.Value;
});

app.MapFallbackToFile("/index.html");

app.Run();
