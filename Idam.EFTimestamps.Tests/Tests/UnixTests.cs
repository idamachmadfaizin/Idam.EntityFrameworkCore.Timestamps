using Idam.EFTimestamps.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Idam.EFTimestamps.Tests.Tests;

public class UnixTests : BaseTest
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_UnixCreate()
    {
        var data = await AddAsync(UnixFaker.Generate());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(UnixMinValue, data.CreatedAt);
        Assert.NotEqual(UnixMinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_UnixUpdate()
    {
        var data = await AddAsync(UnixFaker.Generate());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = UnixFaker.Generate().Name;

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
        var data = await AddAsync(UnixFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Unixs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = await AddRangeAsync(UnixFaker.GenerateLazy(2).ToList());

        await DeleteAsync(datas.First());

        var countUndeleteds = datas.Count(x => !x.DeletedAt.HasValue);

        Assert.True(datas.Count > countUndeleteds);
    }

    [Fact]
    public async Task Should_Restore_Deleted_Unixs()
    {
        var data = await AddAsync(UnixFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Unixs
            .IgnoreQueryFilters()
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
        var data = await AddAsync(UnixFaker.Generate());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        var dataFromDb = await Context.Unixs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.Null(dataFromDb);
    }
}