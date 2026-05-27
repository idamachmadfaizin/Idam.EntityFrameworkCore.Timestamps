using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Context;

public class TestInterceptorDbContext(DbContextOptions<TestInterceptorDbContext> options) : DbContext(options)
{
    public DbSet<Dt> Dts { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddSoftDeleteFilter();

        base.OnModelCreating(modelBuilder);
    }
}
