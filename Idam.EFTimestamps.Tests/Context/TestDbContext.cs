using Idam.EFTimestamps.Extensions;
using Idam.EFTimestamps.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace Idam.EFTimestamps.Tests.Context;

public class TestDbContext : DbContext
{
    public TestDbContext()
    {
    }

    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Dt> Dts { get; init; }
    public DbSet<DtUtc> DtUtcs { get; init; }
    public DbSet<Unix> Unixs { get; init; }
    public DbSet<CreatedAtEntity> CreatedAts { get; init; }
    public DbSet<CreatedAtUnixEntity> CreatedAtUnixs { get; init; }
    public DbSet<CreatedAtUtcEntity> CreatedAtUtcs { get; init; }
    public DbSet<UpdatedAtEntity> UpdatedAts { get; init; }
    public DbSet<UpdatedAtUnixEntity> UpdatedAtUnixs { get; init; }
    public DbSet<UpdatedAtUtcEntity> UpdatedAtUtcs { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("Idam.Libs.EF.Tests");
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ChangeTracker.AddTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        ChangeTracker.AddTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddSoftDeleteFilter();

        base.OnModelCreating(modelBuilder);
    }
}