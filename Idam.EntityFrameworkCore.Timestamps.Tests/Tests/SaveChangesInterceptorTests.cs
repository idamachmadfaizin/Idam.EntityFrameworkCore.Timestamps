using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public class SaveChangesInterceptorTests
{
    [Fact]
    public async Task Should_Set_CreatedAt_And_UpdatedAt_Using_Interceptor()
    {
        await using var context = CreateContext();

        var data = new Dt
        {
            Name = "Dt Name"
        };

        context.Dts.Add(data);
        var created = await context.SaveChangesAsync();

        Assert.True(created > 0);
        Assert.NotEqual(DateTime.MinValue, data.CreatedAt);
        Assert.NotEqual(DateTime.MinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_DeletedAt_Using_Interceptor()
    {
        await using var context = CreateContext();

        var data = new Dt
        {
            Name = "Dt Name"
        };

        context.Dts.Add(data);
        await context.SaveChangesAsync();

        context.Dts.Remove(data);
        var deleted = await context.SaveChangesAsync();

        Assert.True(deleted > 0);

        var dataFromDb = await context.Dts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == data.Id);

        Assert.NotNull(dataFromDb);
        Assert.NotNull(dataFromDb.DeletedAt);
    }

    private static TestInterceptorDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<TestInterceptorDbContext>()
            .UseInMemoryDatabase($"Idam.Libs.EF.Interceptor.Tests.{Guid.NewGuid():N}")
            .AddTimestampsInterceptor()
            .Options;

        return new TestInterceptorDbContext(options);
    }
}
