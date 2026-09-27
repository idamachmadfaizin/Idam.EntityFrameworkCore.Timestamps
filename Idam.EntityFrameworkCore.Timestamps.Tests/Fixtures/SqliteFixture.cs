using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

/// <summary>
///     SQLite needs no container. A temp file is used rather than <c>:memory:</c> so the database
///     does not vanish when the context closes its connection.
/// </summary>
public sealed class SqliteFixture : IDbFixture
{
    public DbContextOptions<TestDbContext> BuildOptions(string databaseName)
    {
        return new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite($"DataSource={Path.Combine(Path.GetTempPath(), $"{databaseName}.db")}")
            .Options;
    }
}

[CollectionDefinition(Name)]
public sealed class SqliteCollection : ICollectionFixture<SqliteFixture>
{
    public const string Name = "Sqlite";
}
