
using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI.Data;

namespace MSChat.ChatAPI.Services;

public class DatabaseMigrationService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseMigrationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
