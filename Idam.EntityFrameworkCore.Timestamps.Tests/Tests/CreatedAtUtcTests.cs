using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class CreatedAtUtcTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Set_Created_At()
    {
        var data = await AddAsync(Fake<CreatedAtUtcEntity>());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(UtcMinValue, data.CreatedAt);
    }

    [Fact]
    public async Task Should_Not_Update_CreatedAt_When_Updated()
    {
        var data = await AddAsync(Fake<CreatedAtUtcEntity>());

        var oldCreatedAt = data.CreatedAt;

        data.Name = Fake<CreatedAtUtcEntity>().Name;

        Context.Update(data);
        await Task.Delay(1);
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.Equal(oldCreatedAt, data.CreatedAt);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class CreatedAtUtcTestsSqlite(SqliteFixture fixture) : CreatedAtUtcTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class CreatedAtUtcTestsMsSql(MsSqlFixture fixture) : CreatedAtUtcTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class CreatedAtUtcTestsMySql(MySqlFixture fixture) : CreatedAtUtcTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class CreatedAtUtcTestsPostgreSql(PostgreSqlFixture fixture) : CreatedAtUtcTests<PostgreSqlFixture>(fixture);
