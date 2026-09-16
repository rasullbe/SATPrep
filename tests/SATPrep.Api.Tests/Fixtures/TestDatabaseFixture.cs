using Microsoft.EntityFrameworkCore;
using SATPrep.Api.Data;

namespace SATPrep.Api.Tests.Fixtures;

public sealed class TestDatabaseFixture
{
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public void ResetDatabase()
    {
        using var context = GetDbContext();
        context.Database.EnsureDeleted();
    }
}