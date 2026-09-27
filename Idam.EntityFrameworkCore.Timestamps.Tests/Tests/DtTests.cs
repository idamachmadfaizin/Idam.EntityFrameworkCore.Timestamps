using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Microsoft.EntityFrameworkCore;

using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class DtTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_DtCreate()
    {
        var data = await AddAsync(Fake<Dt>());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(DateTime.MinValue, data.CreatedAt);
        Assert.NotEqual(DateTime.MinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_DtUpdate()
    {
        var data = await AddAsync(Fake<Dt>());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = Fake<Dt>().Name;

        Context.Update(data);
        await Task.Delay(1);
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.NotEqual(DateTime.MinValue, data.UpdatedAt);
        Assert.NotEqual(oldUpdatedAt, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_DeletedAt_When_DtDelete()
    {
        var data = await AddAsync(Fake<Dt>());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Dts
            .IncludeTrashed()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = await AddRangeAsync(FakeMany<Dt>(2));
        var deleted = await DeleteAsync(datas.First());

        // Query the database: the previous version only counted the in-memory list, which
        // could never fail and never exercised the global query filter.
        var visible = await Context.Dts.ToListAsync();

        Assert.Single(visible);
        Assert.DoesNotContain(visible, x => x.Id == deleted.Id);
    }

    [Fact]
    public async Task Should_Restore_Deleted_Dts()
    {
        var data = await AddAsync(Fake<Dt>());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Dts
            .IncludeTrashed()
            .Where(w => w.Id == data.Id)
            .Where(w => w.DeletedAt.HasValue)
            .FirstOrDefaultAsync();

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);

        Context.Dts.Restore(dataFromDb);
        await Context.SaveChangesAsync();

        dataFromDb = await Context.Dts
            .FirstOrDefaultAsync(x => x.Id == dataFromDb.Id);

        Assert.NotNull(dataFromDb);
        Assert.Null(dataFromDb.DeletedAt);
        Assert.False(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Permanent_Delete_Dts()
    {
        var data = await AddAsync(Fake<Dt>());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Dts
            .IncludeTrashed()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.Null(dataFromDb);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class DtTestsSqlite(SqliteFixture fixture) : DtTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class DtTestsMsSql(MsSqlFixture fixture) : DtTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class DtTestsMySql(MySqlFixture fixture) : DtTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class DtTestsPostgreSql(PostgreSqlFixture fixture) : DtTests<PostgreSqlFixture>(fixture);
