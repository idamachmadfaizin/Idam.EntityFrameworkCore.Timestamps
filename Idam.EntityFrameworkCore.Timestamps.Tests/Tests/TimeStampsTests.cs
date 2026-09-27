using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

/// <summary>
///     Timestamp behaviour that the per-interface suites do not cover: batch consistency, the
///     mixed-interface branch, DateTimeKind, renamed columns, and what the provider stores.
/// </summary>
public abstract class TimeStampsTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Use_One_Timestamp_For_Every_Entity_In_A_Single_Save()
    {
        var datas = await AddRangeAsync(FakeMany<Dt>(50));

        Assert.Single(datas.Select(x => x.CreatedAt).Distinct());
        Assert.Single(datas.Select(x => x.UpdatedAt).Distinct());
    }

    [Fact]
    public async Task Should_Use_One_Timestamp_For_Every_Entity_In_A_Single_Save_Unix()
    {
        var datas = await AddRangeAsync(FakeMany<Unix>(50));

        Assert.Single(datas.Select(x => x.CreatedAt).Distinct());
    }

    [Fact]
    public async Task Should_Set_Local_And_Utc_Separately_For_A_Mixed_Entity()
    {
        var data = await AddAsync(Fake<MixedTimeStamps>());

        Assert.NotEqual(DateTime.MinValue, data.CreatedAt);
        Assert.NotEqual(DateTime.MinValue, data.UpdatedAt);
        Assert.Equal(DateTimeKind.Local, data.CreatedAt.Kind);
        Assert.Equal(DateTimeKind.Utc, data.UpdatedAt.Kind);
    }

    [Fact]
    public async Task Should_Not_Touch_CreatedAt_Of_A_Mixed_Entity_On_Update()
    {
        var data = await AddAsync(Fake<MixedTimeStamps>());
        var createdAt = data.CreatedAt;
        var updatedAt = data.UpdatedAt;

        data.Name = Fake<MixedTimeStamps>().Name;
        Context.Update(data);
        await Task.Delay(1);
        Assert.True(await Context.SaveChangesAsync() > 0);

        Assert.Equal(createdAt, data.CreatedAt);
        Assert.NotEqual(updatedAt, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Use_Local_Kind_For_The_Local_Interfaces()
    {
        var data = await AddAsync(Fake<Dt>());
        var deleted = await DeleteAsync(data);

        Assert.Equal(DateTimeKind.Local, data.CreatedAt.Kind);
        Assert.Equal(DateTimeKind.Local, data.UpdatedAt.Kind);
        Assert.Equal(DateTimeKind.Local, deleted.DeletedAt!.Value.Kind);
    }

    [Fact]
    public async Task Should_Store_Timestamps_In_Renamed_Columns()
    {
        var data = await AddAsync(Fake<RenamedColumns>());

        var stored = await ReloadAsync<RenamedColumns>(data.Id);

        Assert.NotNull(stored);
        Assert.Equal(data.CreatedAt, stored.CreatedAt, TimeSpan.FromSeconds(1));
        Assert.Equal(data.UpdatedAt, stored.UpdatedAt, TimeSpan.FromSeconds(1));
        Assert.Null(stored.DeletedAt);
    }

    [Fact]
    public async Task Should_Persist_Timestamps_Across_A_Roundtrip()
    {
        var local = await AddAsync(Fake<Dt>());
        var utc = await AddAsync(Fake<DtUtc>());
        var unix = await AddAsync(Fake<Unix>());

        var storedLocal = await ReloadAsync<Dt>(local.Id);
        var storedUtc = await ReloadAsync<DtUtc>(utc.Id);
        var storedUnix = await ReloadAsync<Unix>(unix.Id);

        Assert.NotNull(storedLocal);
        Assert.NotNull(storedUtc);
        Assert.NotNull(storedUnix);

        // Kind is deliberately not asserted here: no provider persists it, so a UTC entity
        // comes back as Unspecified. Only the instant itself survives the roundtrip.
        Assert.Equal(local.CreatedAt, storedLocal.CreatedAt, TimeSpan.FromSeconds(1));
        Assert.Equal(utc.CreatedAt, storedUtc.CreatedAt, TimeSpan.FromSeconds(1));
        Assert.Equal(unix.CreatedAt, storedUnix.CreatedAt);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class TimeStampsTestsSqlite(SqliteFixture fixture) : TimeStampsTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class TimeStampsTestsMsSql(MsSqlFixture fixture) : TimeStampsTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class TimeStampsTestsMySql(MySqlFixture fixture) : TimeStampsTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class TimeStampsTestsPostgreSql(PostgreSqlFixture fixture) : TimeStampsTests<PostgreSqlFixture>(fixture);
