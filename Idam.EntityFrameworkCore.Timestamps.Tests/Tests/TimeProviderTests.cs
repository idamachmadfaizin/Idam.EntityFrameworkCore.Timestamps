using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

/// <summary>
///     Where the clock comes from, and what each format makes of it.
/// </summary>
public abstract class TimeProviderTests<TFixture>(TFixture fixture) : BaseTest<TFixture>(fixture)
    where TFixture : IDbFixture
{
    [Fact]
    public async Task Should_Stamp_The_Time_Of_The_Injected_TimeProvider()
    {
        var now = Clock.GetUtcNow();

        var local = await AddAsync(Fake<Dt>());
        var utc = await AddAsync(Fake<DtUtc>());
        var unix = await AddAsync(Fake<Unix>());

        Assert.Equal(now.LocalDateTime, local.CreatedAt);
        Assert.Equal(now.UtcDateTime, utc.CreatedAt);
        Assert.Equal(now.ToUnixTimeMilliseconds(), unix.CreatedAt);

        Clock.Advance(TimeSpan.FromMinutes(1));
        var deleted = await DeleteAsync(local);

        Assert.NotNull(deleted.DeletedAt);
        Assert.Equal(Clock.GetUtcNow().LocalDateTime, deleted.DeletedAt.Value);
    }

    [Fact]
    public async Task Should_Use_The_System_Clock_When_Nothing_Else_Is_Configured()
    {
        await using var context = CreateContext();

        var data = Fake<DtUtc>();
        context.DtUtcs.Add(data);
        await context.SaveChangesAsync();

        Assert.Equal(DateTime.UtcNow, data.CreatedAt, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task Should_Use_The_TimeProvider_Registered_In_The_Application_Services()
    {
        await using var services = new ServiceCollection()
            .AddSingleton<TimeProvider>(Clock)
            .BuildServiceProvider();
        await using var context = CreateContext(o => o.UseApplicationServiceProvider(services));

        var data = Fake<DtUtc>();
        context.DtUtcs.Add(data);
        await context.SaveChangesAsync();

        Assert.Equal(Clock.GetUtcNow().UtcDateTime, data.CreatedAt);
    }

    [Fact]
    public async Task Should_Keep_The_Configured_Clock_When_The_Interceptor_Is_Registered_Twice()
    {
        // The explicit clock is registered first and the parameterless interceptor, added in
        // OnConfiguring, runs last: it must not overwrite the values with the system clock.
        await using var context = CreateContext(o => o.AddTimeStampsInterceptor(Clock));

        var data = Fake<DtUtc>();
        context.DtUtcs.Add(data);
        await context.SaveChangesAsync();

        Assert.Equal(Clock.GetUtcNow().UtcDateTime, data.CreatedAt);
    }

    [Fact]
    public void Should_Read_The_Configured_Clock_From_The_Parameterless_AddTimestamps()
    {
        // What a SaveChanges override calls; it must agree with the interceptor's clock.
        var data = Fake<DtUtc>();
        Context.DtUtcs.Add(data);

        Context.ChangeTracker.AddTimestamps();

        Assert.Equal(Clock.GetUtcNow().UtcDateTime, data.CreatedAt);
    }

    [Fact]
    public async Task Should_Write_Offset_Zero_When_The_TimeProvider_Returns_Another_Offset()
    {
        var plus7 = new DateTimeOffset(2026, 1, 2, 10, 4, 5, TimeSpan.FromHours(7));
        await using var context = CreateContext(o => o.AddTimeStampsInterceptor(new TestClock(plus7)));

        var data = Fake<DtOffset>();
        context.DtOffsets.Add(data);

        // PostgreSQL rejects any offset but zero, so this save is the real check.
        await context.SaveChangesAsync();

        Assert.Equal(plus7.UtcDateTime, data.CreatedAt.UtcDateTime);
        Assert.Equal(TimeSpan.Zero, data.CreatedAt.Offset);
    }

    [Fact]
    public async Task Should_Convert_Local_Time_With_The_TimeProvider_Zone()
    {
        var zone = TimeZoneInfo.CreateCustomTimeZone("Test+07", TimeSpan.FromHours(7), "Test+07", "Test+07");
        var clock = new TestClock(Clock.GetUtcNow()) { LocalZone = zone };
        await using var context = CreateContext(o => o.AddTimeStampsInterceptor(clock));

        var data = Fake<Dt>();
        context.Dts.Add(data);
        await context.SaveChangesAsync();

        Assert.Equal(clock.GetUtcNow().UtcDateTime.AddHours(7), data.CreatedAt);

        // Not the machine's zone, so it cannot honestly claim Kind=Local.
        Assert.Equal(DateTimeKind.Unspecified, data.CreatedAt.Kind);
    }
}

// Runs the suite above against every supported provider. Filter with
// `dotnet test --filter "Provider=Sqlite"` to skip the ones that need a container.
[Trait("Provider", "Sqlite")]
[Collection(SqliteCollection.Name)]
public sealed class TimeProviderTestsSqlite(SqliteFixture fixture) : TimeProviderTests<SqliteFixture>(fixture);

[Trait("Provider", "MsSql")]
[Collection(MsSqlCollection.Name)]
public sealed class TimeProviderTestsMsSql(MsSqlFixture fixture) : TimeProviderTests<MsSqlFixture>(fixture);

[Trait("Provider", "MySql")]
[Collection(MySqlCollection.Name)]
public sealed class TimeProviderTestsMySql(MySqlFixture fixture) : TimeProviderTests<MySqlFixture>(fixture);

[Trait("Provider", "PostgreSql")]
[Collection(PostgreSqlCollection.Name)]
public sealed class TimeProviderTestsPostgreSql(PostgreSqlFixture fixture) : TimeProviderTests<PostgreSqlFixture>(fixture);
