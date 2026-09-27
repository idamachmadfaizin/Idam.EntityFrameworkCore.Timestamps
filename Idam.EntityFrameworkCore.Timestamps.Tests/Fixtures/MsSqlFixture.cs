using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

public sealed class MsSqlFixture : IDbFixture, IAsyncLifetime
{
    // Pinned so a new upstream tag cannot change what CI tests against.
    private readonly MsSqlContainer _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04").Build();

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
        var connectionString = new SqlConnectionStringBuilder(_container.GetConnectionString())
        {
            InitialCatalog = databaseName
        }.ConnectionString;

        return new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlServer(connectionString)
            .Options;
    }
}

[CollectionDefinition(Name)]
public sealed class MsSqlCollection : ICollectionFixture<MsSqlFixture>
{
    public const string Name = "MsSql";
}
