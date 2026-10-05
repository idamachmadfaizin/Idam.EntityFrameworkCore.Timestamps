using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class UpdatedAtUtcTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Set_UpdatedAt_When_Inserted()
    {
        var data = await AddAsync(Fake<UpdatedAtUtcEntity>());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(UtcMinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_UpdatedAt_When_Updated()
    {
        var data = await AddAsync(Fake<UpdatedAtUtcEntity>());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = Fake<UpdatedAtUtcEntity>().Name;

        Context.Update(data);
        Clock.Advance(TimeSpan.FromSeconds(1));
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.NotEqual(oldUpdatedAt, data.UpdatedAt);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class UpdatedAtUtcTestsSqlite(SqliteFixture fixture) : UpdatedAtUtcTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class UpdatedAtUtcTestsMsSql(MsSqlFixture fixture) : UpdatedAtUtcTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class UpdatedAtUtcTestsMySql(MySqlFixture fixture) : UpdatedAtUtcTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class UpdatedAtUtcTestsPostgreSql(PostgreSqlFixture fixture) : UpdatedAtUtcTests<PostgreSqlFixture>(fixture);
