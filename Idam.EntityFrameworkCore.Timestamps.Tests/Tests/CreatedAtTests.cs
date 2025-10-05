namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public class CreatedAtTests : BaseTest
{
    [Fact]
    public async Task Should_Set_Created_At()
    {
        var data = await AddAsync(CreatedAtFaker.Generate());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(DateTime.MinValue, data.CreatedAt);
    }

    [Fact]
    public async Task Should_Not_Update_CreatedAt_When_Updated()
    {
        var data = await AddAsync(CreatedAtFaker.Generate());

        var oldCreatedAt = data.CreatedAt;

        data.Name = CreatedAtFaker.Generate().Name;

        Context.Update(data);
        await Task.Delay(1);
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.Equal(oldCreatedAt, data.CreatedAt);
    }
}