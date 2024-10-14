using Idam.EFTimestamps.Tests.Context;
using Idam.EFTimestamps.Tests.Ekstensions;
using Idam.EFTimestamps.Tests.Faker;
using Microsoft.EntityFrameworkCore;

namespace Idam.EFTimestamps.Tests.Tests;

public abstract class BaseTest
{
    protected readonly TestDbContext _context;
    protected readonly DtFaker _dtFaker;
    protected readonly DtUtcFaker _dtUtcFaker;
    protected readonly UnixFaker _unixFaker;
    protected readonly DateTime _utcMinValue;
    protected readonly long _unixMinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTest"/> class.
    /// </summary>
    public BaseTest()
    {
        _context = new();
        _context.Database.EnsureCreated();

        _dtFaker = new();
        _dtUtcFaker = new();
        _unixFaker = new();

        _utcMinValue = DateTime.MinValue.ToUniversalTime();
        _unixMinValue = DateTime.MinValue.ToUniversalTime().ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Generic add async.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    protected async Task<TEntity> AddAsync<TEntity>(TEntity? data)
        where TEntity : class
    {
        Assert.NotNull(data);

        await _context.Set<TEntity>().AddAsync(data);
        var created = await _context.SaveChangesAsync();

        Assert.True(created > 0);
        return data;
    }

    /// <summary>
    /// Generic Adds the range asynchronous.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="datas">The datas.</param>
    /// <returns></returns>
    protected async Task<IEnumerable<TEntity>> AddRangeAsync<TEntity>(List<TEntity>? datas)
        where TEntity : class
    {
        Assert.NotNull(datas);
        Assert.NotEmpty(datas);

        await _context.Set<TEntity>().AddRangeAsync(datas);
        var created = await _context.SaveChangesAsync();

        Assert.True(created > 0);
        return datas;
    }

    /// <summary>
    /// Generic Delete async.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="data"></param>
    /// <returns></returns>
    protected async Task<TEntity> DeleteAsync<TEntity>(TEntity? data)
        where TEntity : class
    {
        Assert.NotNull(data);

        _context.Set<TEntity>().Remove(data);
        _context.Entry(data).State = EntityState.Deleted;
        var removed = await _context.SaveChangesAsync();

        Assert.True(removed > 0);
        return data;
    }
}