using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

/// <summary>
///     Soft delete behaviour that only a relational provider can prove: query translation,
///     the global filter actually hiding rows, and named filters staying independent.
/// </summary>
public abstract class SoftDeleteTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Return_Only_Trashed_Entities()
    {
        var datas = await AddRangeAsync(FakeMany<Dt>(3));
        var deleted = await DeleteAsync(datas.First());

        var trashed = await Context.Dts.OnlyTrashed().ToListAsync();

        Assert.Single(trashed);
        Assert.Equal(deleted.Id, trashed[0].Id);
        Assert.True(trashed[0].Trashed());
    }

    [Fact]
    public async Task Should_Return_Only_Trashed_Entities_For_Unix()
    {
        var datas = await AddRangeAsync(FakeMany<Unix>(3));
        var deleted = await DeleteAsync(datas.First());

        var trashed = await Context.Unixs.OnlyTrashed().ToListAsync();

        Assert.Single(trashed);
        Assert.Equal(deleted.Id, trashed[0].Id);
    }

    [Fact]
    public async Task Should_Hide_Trashed_Entities_From_A_Plain_Query()
    {
        var datas = await AddRangeAsync(FakeMany<Dt>(3));
        var deleted = await DeleteAsync(datas.First());

        var visible = await Context.Dts.ToListAsync();

        Assert.Equal(2, visible.Count);
        Assert.DoesNotContain(visible, x => x.Id == deleted.Id);
        Assert.Equal(3, await Context.Dts.IncludeTrashed().CountAsync());
    }

    [Fact]
    public async Task Should_Keep_Other_Query_Filters_Active_When_Including_Trashed()
    {
        // Hidden by the tenant filter, never deleted: IncludeTrashed() must not reveal it.
        await AddAsync(new Dt { Name = "hidden", Description = TestDbContext.HiddenTenant });
        var deleted = await DeleteAsync(await AddAsync(Fake<Dt>()));

        var withTrashed = await Context.Dts.IncludeTrashed().ToListAsync();

        Assert.Single(withTrashed);
        Assert.Equal(deleted.Id, withTrashed[0].Id);
        Assert.DoesNotContain(withTrashed, x => x.Description == TestDbContext.HiddenTenant);
    }

    [Fact]
    public async Task Should_Delete_Permanently_When_Force_Removed()
    {
        var data = await AddAsync(Fake<Dt>());
        Clock.Advance(TimeSpan.FromMinutes(1));

        Context.Dts.ForceRemove(data);

        // Same clock as the interceptor, in case the marker outlives a failed save.
        Assert.Equal(Clock.GetUtcNow().LocalDateTime, data.DeletedAt);
        await Context.SaveChangesAsync();

        Assert.Null(await ReloadAsync<Dt>(data.Id));
    }

    [Fact]
    public async Task Should_Restore_An_Entity_That_Is_Not_Tracked()
    {
        var data = await DeleteAsync(await AddAsync(Fake<Dt>()));

        var detached = await ReloadAsync<Dt>(data.Id);
        Assert.NotNull(detached);
        Assert.NotNull(detached.DeletedAt);

        Context.Dts.Restore(detached);
        Assert.True(await Context.SaveChangesAsync() > 0);

        var restored = await ReloadAsync<Dt>(data.Id);
        Assert.NotNull(restored);
        Assert.Null(restored.DeletedAt);
    }

    [Fact]
    public async Task Should_Soft_Delete_An_Entity_Without_Any_TimeStamp_Interface()
    {
        var data = await DeleteAsync(await AddAsync(Fake<SoftDeleteOnly>()));

        var stored = await ReloadAsync<SoftDeleteOnly>(data.Id);

        Assert.NotNull(stored);
        Assert.NotNull(stored.DeletedAt);
        Assert.Empty(await Context.SoftDeleteOnlys.ToListAsync());
    }

    [Fact]
    public async Task Should_Soft_Delete_A_Derived_Type_In_A_Tph_Hierarchy()
    {
        var dog = await AddAsync(Fake<Dog>());
        await DeleteAsync(dog);

        var stored = await ReloadAsync<Animal>(dog.Id);

        Assert.NotNull(stored);
        Assert.NotNull(stored.DeletedAt);

        // The filter lives on the root, so it must hide the row from both sets.
        Assert.Empty(await Context.Dogs.ToListAsync());
        Assert.Empty(await Context.Animals.ToListAsync());
        Assert.Single(await Context.Animals.IncludeTrashed().ToListAsync());
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class SoftDeleteTestsSqlite(SqliteFixture fixture) : SoftDeleteTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class SoftDeleteTestsMsSql(MsSqlFixture fixture) : SoftDeleteTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class SoftDeleteTestsMySql(MySqlFixture fixture) : SoftDeleteTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class SoftDeleteTestsPostgreSql(PostgreSqlFixture fixture) : SoftDeleteTests<PostgreSqlFixture>(fixture);
