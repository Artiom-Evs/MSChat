var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOrchardCms()
    .AddDatabaseShellsConfiguration();

var app = builder.Build();

app.UseStaticFiles();
app.UseOrchardCore();

app.Run();
