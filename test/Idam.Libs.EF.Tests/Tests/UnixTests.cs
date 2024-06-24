using Idam.Libs.EF.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Idam.Libs.EF.Tests.Tests;
public class UnixTests : BaseTest
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_UnixCreate()
    {
        var data = await AddAsync(_unixFaker.Generate());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(_unixMinValue, data.CreatedAt);
        Assert.NotEqual(_unixMinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_UnixUpdate()
    {
        var data = await AddAsync(_unixFaker.Generate());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = _unixFaker.Generate().Name;

        _context.Update(data);
        var updated = await _context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.NotEqual(_unixMinValue, data.UpdatedAt);
        Assert.NotEqual(oldUpdatedAt, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_DeletedAt_When_UnixDelete()
    {
        var data = await AddAsync(_unixFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await _context.Unixs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = await AddRangeAsync(_unixFaker.GenerateLazy(2).ToList());

        await DeleteAsync(datas.First());

        var countUndeleteds = datas.Where(x => !x.DeletedAt.HasValue).Count();

        Assert.True(datas.Count() > countUndeleteds);
    }

    [Fact]
    public async Task Should_Restore_Deleted_Unixs()
    {
        var data = await AddAsync(_unixFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await _context.Unixs
            .IgnoreQueryFilters()
            .Where(w => w.Id == data.Id)
            .Where(w => w.DeletedAt.HasValue)
            .FirstOrDefaultAsync();

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);

        _context.Unixs.Restore(dataFromDb);
        await _context.SaveChangesAsync();

        dataFromDb = await _context.Unixs
            .FirstOrDefaultAsync(x => x.Id == dataFromDb.Id);

        Assert.NotNull(dataFromDb);
        Assert.Null(dataFromDb.DeletedAt);
        Assert.False(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Permanent_Delete_Unixs()
    {
        var data = await AddAsync(_unixFaker.Generate());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        var dataFromDb = await _context.Unixs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.Null(dataFromDb);
    }
}
