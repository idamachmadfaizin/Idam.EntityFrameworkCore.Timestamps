using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Idam.EntityFrameworkCore.Timestamps.Tests.Ekstensions;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Idam.EntityFrameworkCore.Timestamps.Tests.Faker;
using Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

/// <summary>
///     Base class for every test. The fixture decides which provider the test runs against;
///     each test still gets its own database so the tests stay isolated from one another.
/// </summary>
/// <typeparam name="TFixture">The provider fixture.</typeparam>
public abstract class BaseTest<TFixture> : IDisposable
    where TFixture : IDbFixture
{
    protected readonly TestDbContext Context;
    protected readonly long UnixMinValue;
    protected readonly DateTime UtcMinValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseTest{TFixture}" /> class.
    /// </summary>
    /// <param name="fixture">The provider fixture.</param>
    protected BaseTest(TFixture fixture)
    {
        // ponytail: one database per test keeps the original isolation, but on SQL Server a
        // CREATE/DROP DATABASE per test is the slowest part of the run. Swap for a per-class
        // database plus row cleanup if the suite gets too slow.
        Context = new TestDbContext(fixture.BuildOptions($"timestamps_test_{Guid.NewGuid():N}"));
        Context.Database.EnsureCreated();

        UtcMinValue = DateTime.MinValue.ToUniversalTime();
        UnixMinValue = DateTime.MinValue.ToUniversalTime().ToUnixTimeMilliseconds();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Generates one fake entity.
    /// </summary>
    protected static TEntity Fake<TEntity>()
        where TEntity : BaseEntity
    {
        return new BaseEntityFaker<TEntity>().Generate();
    }

    /// <summary>
    ///     Generates a list of fake entities.
    /// </summary>
    protected static List<TEntity> FakeMany<TEntity>(int count)
        where TEntity : BaseEntity
    {
        return new BaseEntityFaker<TEntity>().Generate(count);
    }

    /// <summary>
    ///     Reads an entity back from the database, bypassing the change tracker, so that what the
    ///     provider actually stored is asserted rather than the in-memory instance.
    /// </summary>
    protected async Task<TEntity?> ReloadAsync<TEntity>(int id)
        where TEntity : BaseEntity
    {
        Context.ChangeTracker.Clear();

        return await Context.Set<TEntity>()
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    ///     Generic add async.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    protected async Task<TEntity> AddAsync<TEntity>(TEntity? data)
        where TEntity : class
    {
        Assert.NotNull(data);

        await Context.Set<TEntity>().AddAsync(data);
        var created = await Context.SaveChangesAsync();

        Assert.True(created > 0);
        return data;
    }

    /// <summary>
    ///     Generic Adds the range asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="datas">The datas.</param>
    /// <returns></returns>
    protected async Task<IList<TEntity>> AddRangeAsync<TEntity>(List<TEntity>? datas)
        where TEntity : class
    {
        Assert.NotNull(datas);
        Assert.NotEmpty(datas);

        await Context.Set<TEntity>().AddRangeAsync(datas);
        var created = await Context.SaveChangesAsync();

        Assert.True(created > 0);
        return datas;
    }

    /// <summary>
    ///     Generic Delete async.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    protected async Task<TEntity> DeleteAsync<TEntity>(TEntity? data)
        where TEntity : class
    {
        Assert.NotNull(data);

        Context.Set<TEntity>().Remove(data);
        Context.Entry(data).State = EntityState.Deleted;
        var removed = await Context.SaveChangesAsync();

        Assert.True(removed > 0);
        return data;
    }
}
