using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class DtOffsetTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    /// <summary>
    ///     How close a stored sub-second value must come back: the column's microsecond precision.
    /// </summary>
    protected virtual TimeSpan StoredPrecision => TimeSpan.FromMicroseconds(1);

    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_DtOffsetCreate()
    {
        var data = await AddAsync(Fake<DtOffset>());

        Assert.NotEqual(0, data.Id);
        Assert.Equal(Clock.GetUtcNow(), data.CreatedAt);
        Assert.Equal(Clock.GetUtcNow(), data.UpdatedAt);
        Assert.Equal(TimeSpan.Zero, data.CreatedAt.Offset);
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_DtOffsetUpdate()
    {
        var data = await AddAsync(Fake<DtOffset>());
        var createdAt = data.CreatedAt;

        data.Name = Fake<DtOffset>().Name;

        Context.Update(data);
        Clock.Advance(TimeSpan.FromSeconds(1));
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.Equal(createdAt, data.CreatedAt);
        Assert.Equal(Clock.GetUtcNow(), data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_DeletedAt_When_DtOffsetDelete()
    {
        var data = await AddAsync(Fake<DtOffset>());
        Clock.Advance(TimeSpan.FromSeconds(1));
        data = await DeleteAsync(data);

        var dataFromDb = await ReloadAsync<DtOffset>(data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
        Assert.Equal(Clock.GetUtcNow(), dataFromDb.DeletedAt.Value);
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = await AddRangeAsync(FakeMany<DtOffset>(2));
        var deleted = await DeleteAsync(datas.First());

        var visible = await Context.DtOffsets.ToListAsync();

        Assert.Single(visible);
        Assert.DoesNotContain(visible, x => x.Id == deleted.Id);
    }

    [Fact]
    public async Task Should_Return_Only_Trashed_Entities()
    {
        var datas = await AddRangeAsync(FakeMany<DtOffset>(3));
        var deleted = await DeleteAsync(datas.First());

        var trashed = await Context.DtOffsets.OnlyTrashed().ToListAsync();

        Assert.Single(trashed);
        Assert.Equal(deleted.Id, trashed[0].Id);
    }

    [Fact]
    public async Task Should_Restore_Deleted_DtOffsets()
    {
        var data = await DeleteAsync(await AddAsync(Fake<DtOffset>()));

        var detached = await ReloadAsync<DtOffset>(data.Id);
        Assert.NotNull(detached);
        Assert.NotNull(detached.DeletedAt);

        Context.DtOffsets.Restore(detached);
        await Context.SaveChangesAsync();

        var restored = await Context.DtOffsets.FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(restored);
        Assert.Null(restored.DeletedAt);
        Assert.False(restored.Trashed());
    }

    [Fact]
    public async Task Should_Permanent_Delete_DtOffsets()
    {
        var data = await AddAsync(Fake<DtOffset>());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        Assert.Null(await ReloadAsync<DtOffset>(data.Id));
    }

    [Fact]
    public async Task Should_Delete_Permanently_When_Force_Removed()
    {
        var data = await AddAsync(Fake<DtOffset>());

        Context.DtOffsets.ForceRemove(data);
        await Context.SaveChangesAsync();

        Assert.Null(await ReloadAsync<DtOffset>(data.Id));
    }

    [Fact]
    public async Task Should_Persist_The_Instant_And_Offset_Across_A_Roundtrip()
    {
        var data = await DeleteAsync(await AddAsync(Fake<DtOffset>()));

        var stored = await ReloadAsync<DtOffset>(data.Id);

        Assert.NotNull(stored);
        Assert.NotNull(stored.DeletedAt);

        // DateTimeOffset equality compares instants only, so the offset is asserted separately.
        Assert.Equal(Clock.GetUtcNow(), stored.CreatedAt);
        Assert.Equal(Clock.GetUtcNow(), stored.UpdatedAt);
        Assert.Equal(Clock.GetUtcNow(), stored.DeletedAt.Value);
        Assert.Equal(TimeSpan.Zero, stored.CreatedAt.Offset);
        Assert.Equal(TimeSpan.Zero, stored.UpdatedAt.Offset);
        Assert.Equal(TimeSpan.Zero, stored.DeletedAt.Value.Offset);
    }

    [Fact]
    public async Task Should_Persist_A_Sub_Second_Instant_To_The_Column_Precision()
    {
        // 100ns ticks, finer than the microsecond columns of SQL Server, MySQL and PostgreSQL.
        Clock.Advance(TimeSpan.FromTicks(1_234_567));
        var data = await AddAsync(Fake<DtOffset>());

        var stored = await ReloadAsync<DtOffset>(data.Id);

        Assert.NotNull(stored);
        Assert.Equal(data.CreatedAt, stored.CreatedAt, StoredPrecision);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class DtOffsetTestsSqlite(SqliteFixture fixture) : DtOffsetTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class DtOffsetTestsMsSql(MsSqlFixture fixture) : DtOffsetTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class DtOffsetTestsMySql(MySqlFixture fixture) : DtOffsetTests<MySqlFixture>(fixture)
{
    // Known limitation: MySql.EntityFrameworkCore drops the fraction of a DateTimeOffset, even in a
    // datetime(6) column. DateTime keeps it. See the README for the converter that works around it.
    protected override TimeSpan StoredPrecision => TimeSpan.FromSeconds(1);
}

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class DtOffsetTestsPostgreSql(PostgreSqlFixture fixture) : DtOffsetTests<PostgreSqlFixture>(fixture);
