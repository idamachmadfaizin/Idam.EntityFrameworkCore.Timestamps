using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class CreatedAtTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Set_Created_At()
    {
        var data = await AddAsync(Fake<CreatedAtEntity>());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(DateTime.MinValue, data.CreatedAt);
    }

    [Fact]
    public async Task Should_Not_Update_CreatedAt_When_Updated()
    {
        var data = await AddAsync(Fake<CreatedAtEntity>());

        var oldCreatedAt = data.CreatedAt;

        data.Name = Fake<CreatedAtEntity>().Name;

        Context.Update(data);
        Clock.Advance(TimeSpan.FromSeconds(1));
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.Equal(oldCreatedAt, data.CreatedAt);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class CreatedAtTestsSqlite(SqliteFixture fixture) : CreatedAtTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class CreatedAtTestsMsSql(MsSqlFixture fixture) : CreatedAtTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class CreatedAtTestsMySql(MySqlFixture fixture) : CreatedAtTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class CreatedAtTestsPostgreSql(PostgreSqlFixture fixture) : CreatedAtTests<PostgreSqlFixture>(fixture);
