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

    public DbSet<Dt> Dts { get; set; }
    public DbSet<DtUtc> DtUtcs { get; set; }
    public DbSet<Unix> Unixs { get; set; }
    public DbSet<CreatedAtEntity> CreatedAts { get; set; }
    public DbSet<CreatedAtUnixEntity> CreatedAtUnixs { get; set; }
    public DbSet<CreatedAtUtcEntity> CreatedAtUtcs { get; set; }
    public DbSet<UpdatedAtEntity> UpdatedAts { get; set; }
    public DbSet<UpdatedAtUnixEntity> UpdatedAtUnixs { get; set; }
    public DbSet<UpdatedAtUtcEntity> UpdatedAtUtcs { get; set; }

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