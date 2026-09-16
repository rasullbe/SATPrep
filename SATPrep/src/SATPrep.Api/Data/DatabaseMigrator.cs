using Microsoft.EntityFrameworkCore;

namespace SATPrep.Api.Data;

public static class DatabaseMigrator
{
    public static async Task ApplyAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
    }
}