using Idam.Libs.EF.Extensions;
using Idam.Libs.EF.Sample.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Idam.Libs.EF.Sample.Context;

public class MyDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Unix> Unixs { get; set; }
    public DbSet<Dt> Dts { get; set; }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ChangeTracker.AddTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
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
