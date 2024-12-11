namespace Idam.EFTimestamps.Tests.Tests;

public class CreatedAtUnixTests : BaseTest
{
    [Fact]
    public async Task Should_Set_Created_At()
    {
        var data = await AddAsync(CreatedAtUnixFaker.Generate());

        Assert.NotEqual(0, data.Id);
        Assert.NotEqual(UnixMinValue, data.CreatedAt);
    }

    [Fact]
    public async Task Should_Not_Update_CreatedAt_When_Updated()
    {
        var data = await AddAsync(CreatedAtUnixFaker.Generate());

        var oldCreatedAt = data.CreatedAt;

        data.Name = CreatedAtUnixFaker.Generate().Name;

        Context.Update(data);
        await Task.Delay(1);
        var updated = await Context.SaveChangesAsync();

        Assert.True(updated > 0);
        Assert.Equal(oldCreatedAt, data.CreatedAt);
    }
}