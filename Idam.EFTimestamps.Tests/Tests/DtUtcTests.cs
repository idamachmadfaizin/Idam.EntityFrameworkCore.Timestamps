using Idam.EFTimestamps.Extensions;
using Idam.EFTimestamps.Tests.Ekstensions;
using Microsoft.EntityFrameworkCore;

namespace Idam.EFTimestamps.Tests.Tests;
public class DtUtcTests : BaseTest
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_DtUtcCreate()
    {
        var data = await AddAsync(_dtUtcFaker.Generate());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(_utcMinValue, data.CreatedAt);
        Assert.NotEqual(_utcMinValue, data.UpdatedAt);
        Assert.True(data.CreatedAt.IsUtc());
        Assert.True(data.UpdatedAt.IsUtc());
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_DtUtcUpdate()
    {
        var data = await AddAsync(_dtUtcFaker.Generate());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = _dtUtcFaker.Generate().Name;

        _context.Update(data);
        await Task.Delay(1);
        var updated = await _context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.NotEqual(_utcMinValue, data.UpdatedAt);
        Assert.NotEqual(oldUpdatedAt, data.UpdatedAt);
        Assert.True(data.UpdatedAt.IsUtc());
    }

    [Fact]
    public async Task Should_Set_DeletedAt_When_DtUtcDelete()
    {
        var data = await AddAsync(_dtUtcFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await _context.DtUtcs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
        Assert.True(dataFromDb.DeletedAt?.IsUtc());
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = (await AddRangeAsync(_dtUtcFaker.GenerateLazy(2).ToList())).ToArray();

        await DeleteAsync(datas.First());

        var countUndelete = datas.Count(x => !x.DeletedAt.HasValue);

        Assert.True(datas.Length > countUndelete);
    }

    [Fact]
    public async Task Should_Restore_Deleted_Dts()
    {
        var data = await AddAsync(_dtUtcFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await _context.DtUtcs
            .IgnoreQueryFilters()
            .Where(w => w.Id == data.Id)
            .Where(w => w.DeletedAt.HasValue)
            .FirstOrDefaultAsync();

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);

        _context.DtUtcs.Restore(dataFromDb);
        await _context.SaveChangesAsync();

        dataFromDb = await _context.DtUtcs
            .FirstOrDefaultAsync(x => x.Id == dataFromDb.Id);

        Assert.NotNull(dataFromDb);
        Assert.Null(dataFromDb.DeletedAt);
        Assert.False(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Permanent_Delete_Dts()
    {
        var data = await AddAsync(_dtUtcFaker.Generate());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        var dataFromDb = await _context.DtUtcs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.Null(dataFromDb);
    }
}
