using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public class DtTests : BaseTest
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_DtCreate()
    {
        var data = await AddAsync(DtFaker.Generate());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(DateTime.MinValue, data.CreatedAt);
        Assert.NotEqual(DateTime.MinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_DtUpdate()
    {
        var data = await AddAsync(DtFaker.Generate());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = DtFaker.Generate().Name;

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
        var data = await AddAsync(DtFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Dts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = await AddRangeAsync(DtFaker.GenerateLazy(2).ToList());

        await DeleteAsync(datas.First());

        var activeDatas = await Context.Dts.ToListAsync();
        var deletedDatas = await Context.Dts
            .IgnoreQueryFilters()
            .WhereTrashed()
            .ToListAsync();

        Assert.Single(activeDatas);
        Assert.Single(deletedDatas);
    }

    [Fact]
    public async Task Should_Query_Active_And_Deleted_With_Query_Helpers()
    {
        var datas = await AddRangeAsync(DtFaker.GenerateLazy(2).ToList());

        await Context.Dts.SoftDeleteAsync(datas.First());

        var activeCount = await Context.Dts
            .IgnoreQueryFilters()
            .WhereActive()
            .CountAsync();

        var deletedCount = await Context.Dts
            .IgnoreQueryFilters()
            .WhereTrashed()
            .CountAsync();

        Assert.Equal(1, activeCount);
        Assert.Equal(1, deletedCount);
    }

    [Fact]
    public async Task Should_SoftDelete_And_Restore_With_Async_Helpers()
    {
        var data = await AddAsync(DtFaker.Generate());

        var softDeleted = await Context.Dts.SoftDeleteAsync(data);
        Assert.True(softDeleted > 0);

        var deletedData = await Context.Dts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(deletedData);
        Assert.NotNull(deletedData.DeletedAt);

        var restored = await Context.Dts.RestoreAsync(deletedData);
        Assert.True(restored > 0);

        var restoredData = await Context.Dts
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(restoredData);
        Assert.Null(restoredData.DeletedAt);
    }

    [Fact]
    public async Task Should_Restore_Deleted_Dts()
    {
        var data = await AddAsync(DtFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Dts
            .IgnoreQueryFilters()
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
        var data = await AddAsync(DtFaker.Generate());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Dts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.Null(dataFromDb);
    }
}
