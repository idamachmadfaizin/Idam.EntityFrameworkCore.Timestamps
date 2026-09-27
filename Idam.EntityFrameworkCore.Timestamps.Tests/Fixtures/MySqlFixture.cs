using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Testcontainers.MySql;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

public sealed class MySqlFixture : IDbFixture, IAsyncLifetime
{
    // The module's default user may not CREATE DATABASE, and every test needs its own.
    private readonly MySqlContainer _container = new MySqlBuilder("mysql:8.0")
        .WithUsername("root")
        .WithPassword("root")
        .Build();

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
        var connectionString = new MySqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Database = databaseName
        }.ConnectionString;

        return new DbContextOptionsBuilder<TestDbContext>()
            .UseMySQL(connectionString)
            .Options;
    }
}

[CollectionDefinition(Name)]
public sealed class MySqlCollection : ICollectionFixture<MySqlFixture>
{
    public const string Name = "MySql";
}
