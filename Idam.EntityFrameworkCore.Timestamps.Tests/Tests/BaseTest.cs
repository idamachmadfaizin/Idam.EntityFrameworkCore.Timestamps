using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Idam.EntityFrameworkCore.Timestamps.Tests.Ekstensions;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Idam.EntityFrameworkCore.Timestamps.Tests.Faker;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Tests;

public abstract class BaseTest
{
    protected readonly TestDbContext Context;

    protected readonly BaseEntityFaker<CreatedAtEntity> CreatedAtFaker;
    protected readonly BaseEntityFaker<CreatedAtUnixEntity> CreatedAtUnixFaker;
    protected readonly BaseEntityFaker<CreatedAtUtcEntity> CreatedAtUtcFaker;
    protected readonly BaseEntityFaker<Dt> DtFaker;
    protected readonly BaseEntityFaker<DtUtc> DtUtcFaker;
    protected readonly BaseEntityFaker<Unix> UnixFaker;
    protected readonly long UnixMinValue;
    protected readonly BaseEntityFaker<UpdatedAtEntity> UpdatedAtFaker;
    protected readonly BaseEntityFaker<UpdatedAtUnixEntity> UpdatedAtUnixFaker;
    protected readonly BaseEntityFaker<UpdatedAtUtcEntity> UpdatedAtUtcFaker;

    protected readonly DateTime UtcMinValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseTest" /> class.
    /// </summary>
    protected BaseTest()
    {
        Context = new TestDbContext();
        Context.Database.EnsureCreated();

        DtFaker = new BaseEntityFaker<Dt>();
        CreatedAtFaker = new BaseEntityFaker<CreatedAtEntity>();
        CreatedAtUnixFaker = new BaseEntityFaker<CreatedAtUnixEntity>();
        CreatedAtUtcFaker = new BaseEntityFaker<CreatedAtUtcEntity>();
        DtFaker = new BaseEntityFaker<Dt>();
        DtUtcFaker = new BaseEntityFaker<DtUtc>();
        UnixFaker = new BaseEntityFaker<Unix>();
        UpdatedAtFaker = new BaseEntityFaker<UpdatedAtEntity>();
        UpdatedAtUnixFaker = new BaseEntityFaker<UpdatedAtUnixEntity>();
        UpdatedAtUtcFaker = new BaseEntityFaker<UpdatedAtUtcEntity>();

        UtcMinValue = DateTime.MinValue.ToUniversalTime();
        UnixMinValue = DateTime.MinValue.ToUniversalTime().ToUnixTimeMilliseconds();
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

//TODO: Create a tests for new entities.