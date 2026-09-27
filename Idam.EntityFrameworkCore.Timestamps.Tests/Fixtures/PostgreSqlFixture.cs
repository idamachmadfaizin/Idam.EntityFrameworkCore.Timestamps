using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

public sealed class PostgreSqlFixture : IDbFixture, IAsyncLifetime
{
    // Pinned so a new upstream tag cannot change what CI tests against.
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:15.1").Build();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    public DbContextOptions<TestDbContext> BuildOptions(string databaseName)
    {
        var connectionString = new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Database = databaseName
        }.ConnectionString;

        return new DbContextOptionsBuilder<TestDbContext>()
            .UseNpgsql(connectionString)
            .Options;
    }
}

[CollectionDefinition(Name)]
public sealed class PostgreSqlCollection : ICollectionFixture<PostgreSqlFixture>
{
    public const string Name = "PostgreSql";
}
