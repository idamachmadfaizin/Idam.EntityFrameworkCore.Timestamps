using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Idam.EntityFrameworkCore.Timestamps.Tests.Extensions;
using Microsoft.EntityFrameworkCore;

using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class DtUtcTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_DtUtcCreate()
    {
        var data = await AddAsync(Fake<DtUtc>());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(UtcMinValue, data.CreatedAt);
        Assert.NotEqual(UtcMinValue, data.UpdatedAt);
        Assert.True(data.CreatedAt.IsUtc());
        Assert.True(data.UpdatedAt.IsUtc());
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_DtUtcUpdate()
    {
        var data = await AddAsync(Fake<DtUtc>());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = Fake<DtUtc>().Name;

        Context.Update(data);
        Clock.Advance(TimeSpan.FromSeconds(1));
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.NotEqual(UtcMinValue, data.UpdatedAt);
        Assert.NotEqual(oldUpdatedAt, data.UpdatedAt);
        Assert.True(data.UpdatedAt.IsUtc());
    }

    [Fact]
    public async Task Should_Set_DeletedAt_When_DtUtcDelete()
    {
        var data = await AddAsync(Fake<DtUtc>());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.DtUtcs
            .IncludeTrashed()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
        Assert.True(dataFromDb.DeletedAt?.IsUtc());
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = await AddRangeAsync(FakeMany<DtUtc>(2));
        var deleted = await DeleteAsync(datas.First());

        // Query the database: the previous version only counted the in-memory list, which
        // could never fail and never exercised the global query filter.
        var visible = await Context.DtUtcs.ToListAsync();

        Assert.Single(visible);
        Assert.DoesNotContain(visible, x => x.Id == deleted.Id);
    }

    [Fact]
    public async Task Should_Restore_Deleted_Dts()
    {
        var data = await AddAsync(Fake<DtUtc>());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.DtUtcs
            .IncludeTrashed()
            .Where(w => w.Id == data.Id)
            .Where(w => w.DeletedAt.HasValue)
            .FirstOrDefaultAsync();

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);

        Context.DtUtcs.Restore(dataFromDb);
        await Context.SaveChangesAsync();

        dataFromDb = await Context.DtUtcs
            .FirstOrDefaultAsync(x => x.Id == dataFromDb.Id);

        Assert.NotNull(dataFromDb);
        Assert.Null(dataFromDb.DeletedAt);
        Assert.False(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Permanent_Delete_Dts()
    {
        var data = await AddAsync(Fake<DtUtc>());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        var dataFromDb = await Context.DtUtcs
            .IncludeTrashed()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.Null(dataFromDb);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class DtUtcTestsSqlite(SqliteFixture fixture) : DtUtcTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class DtUtcTestsMsSql(MsSqlFixture fixture) : DtUtcTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class DtUtcTestsMySql(MySqlFixture fixture) : DtUtcTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class DtUtcTestsPostgreSql(PostgreSqlFixture fixture) : DtUtcTests<PostgreSqlFixture>(fixture);
