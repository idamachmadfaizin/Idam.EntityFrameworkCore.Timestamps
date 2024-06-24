using Idam.Libs.EF.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Idam.Libs.EF.Tests.Tests;
public class DtTests : BaseTest
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_When_DtCreate()
    {
        var data = await AddAsync(_dtFaker.Generate());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(_utcMinValue, data.CreatedAt);
        Assert.NotEqual(_utcMinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Update_UpdatedAt_When_DtUpdate()
    {
        var data = await AddAsync(_dtFaker.Generate());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = _dtFaker.Generate().Name;

        _context.Update(data);
        var updated = await _context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.NotEqual(_utcMinValue, data.UpdatedAt);
        Assert.NotEqual(oldUpdatedAt, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_DeletedAt_When_DtDelete()
    {
        var data = await AddAsync(_dtFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await _context.Dts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
        Assert.True(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Filtered_Not_Null_DeletedAt_From_List()
    {
        var datas = await AddRangeAsync(_dtFaker.GenerateLazy(2).ToList());

        await DeleteAsync(datas.First());

        var countUndeleteds = datas.Where(x => !x.DeletedAt.HasValue).Count();

        Assert.True(datas.Count() > countUndeleteds);
    }

    [Fact]
    public async Task Should_Restore_Deleted_Dts()
    {
        var data = await AddAsync(_dtFaker.Generate());
        data = await DeleteAsync(data);

        var dataFromDb = await _context.Dts
            .IgnoreQueryFilters()
            .Where(w => w.Id == data.Id)
            .Where(w => w.DeletedAt.HasValue)
            .FirstOrDefaultAsync();

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);

        _context.Dts.Restore(dataFromDb);
        await _context.SaveChangesAsync();

        dataFromDb = await _context.Dts
            .FirstOrDefaultAsync(x => x.Id == dataFromDb.Id);

        Assert.NotNull(dataFromDb);
        Assert.Null(dataFromDb.DeletedAt);
        Assert.False(dataFromDb.Trashed());
    }

    [Fact]
    public async Task Should_Permanent_Delete_Dts()
    {
        var data = await AddAsync(_dtFaker.Generate());
        data = await DeleteAsync(data);
        data = await DeleteAsync(data);

        var dataFromDb = await _context.Dts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.Null(dataFromDb);
    }
}
