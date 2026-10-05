using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class UpdatedAtUnixTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Set_UpdatedAt_When_Inserted()
    {
        var data = await AddAsync(Fake<UpdatedAtUnixEntity>());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(UnixMinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_UpdatedAt_When_Updated()
    {
        var data = await AddAsync(Fake<UpdatedAtUnixEntity>());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = Fake<UpdatedAtUnixEntity>().Name;

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
public sealed class UpdatedAtUnixTestsSqlite(SqliteFixture fixture) : UpdatedAtUnixTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class UpdatedAtUnixTestsMsSql(MsSqlFixture fixture) : UpdatedAtUnixTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class UpdatedAtUnixTestsMySql(MySqlFixture fixture) : UpdatedAtUnixTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class UpdatedAtUnixTestsPostgreSql(PostgreSqlFixture fixture) : UpdatedAtUnixTests<PostgreSqlFixture>(fixture);
