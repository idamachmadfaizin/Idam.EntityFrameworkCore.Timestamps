namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public class UpdatedAtUtcTests : BaseTest
{
    [Fact]
    public async Task Should_Set_UpdatedAt_When_Inserted()
    {
        var data = await AddAsync(UpdatedAtUtcFaker.Generate());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(UtcMinValue, data.UpdatedAt);
    }

    [Fact]
    public async Task Should_Set_UpdatedAt_When_Updated()
    {
        var data = await AddAsync(UpdatedAtUtcFaker.Generate());

        var oldUpdatedAt = data.UpdatedAt;

        data.Name = UpdatedAtUtcFaker.Generate().Name;

        Context.Update(data);
        await Task.Delay(1);
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.NotEqual(oldUpdatedAt, data.UpdatedAt);
    }
}