using Idam.EFTimestamps.Extensions;
using Idam.EFTimestamps.Sample.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Idam.EFTimestamps.Sample.Context;

public class MyDbContext(DbContextOptions options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<Unix> Unixs { get; init; }
    public DbSet<Dt> Dts { get; init; }
    public DbSet<DtUtc> DtUtcs { get; init; }
    public DbSet<DtCustom> DtCustoms { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
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