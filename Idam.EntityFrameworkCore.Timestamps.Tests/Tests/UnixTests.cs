using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Microsoft.EntityFrameworkCore;

using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class UnixTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_UnixCreate()
    {
        var data = await AddAsync(Fake<Unix>());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(UnixMinValue, data.CreatedAt);
        Assert.NotEqual(UnixMinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_UnixUpdate()
    {
        var data = await AddAsync(Fake<Unix>());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = Fake<Unix>().Name;

        Context.Update(data);
        await Task.Delay(1);
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.NotEqual(UnixMinValue, data.UpdatedAt);
        Assert.NotEqual(oldUpdatedAt, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_DeletedAt_When_UnixDelete()
    {
        var data = await AddAsync(Fake<Unix>());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Unixs
            .IncludeTrashed()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = await AddRangeAsync(FakeMany<Unix>(2));
        var deleted = await DeleteAsync(datas.First());

        // Query the database: the previous version only counted the in-memory list, which
        // could never fail and never exercised the global query filter.
        var visible = await Context.Unixs.ToListAsync();

        Assert.Single(visible);
        Assert.DoesNotContain(visible, x => x.Id == deleted.Id);
    }

    [Fact]
    public async Task Should_Restore_Deleted_Unixs()
    {
        var data = await AddAsync(Fake<Unix>());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Unixs
            .IncludeTrashed()
            .Where(w => w.Id == data.Id)
            .Where(w => w.DeletedAt.HasValue)
            .FirstOrDefaultAsync();

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);

        Context.Unixs.Restore(dataFromDb);
        await Context.SaveChangesAsync();

        dataFromDb = await Context.Unixs
            .FirstOrDefaultAsync(x => x.Id == dataFromDb.Id);

        Assert.NotNull(dataFromDb);
        Assert.Null(dataFromDb.DeletedAt);
        Assert.False(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Permanent_Delete_Unixs()
    {
        var data = await AddAsync(Fake<Unix>());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Unixs
            .IncludeTrashed()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.Null(dataFromDb);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class UnixTestsSqlite(SqliteFixture fixture) : UnixTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class UnixTestsMsSql(MsSqlFixture fixture) : UnixTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class UnixTestsMySql(MySqlFixture fixture) : UnixTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class UnixTestsPostgreSql(PostgreSqlFixture fixture) : UnixTests<PostgreSqlFixture>(fixture);
