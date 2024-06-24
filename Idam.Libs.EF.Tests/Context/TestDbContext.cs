using Idam.Libs.EF.Extensions;
using Idam.Libs.EF.Sample.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Idam.Libs.EF.Tests.Context;
public class TestDbContext : DbContext
{
    public TestDbContext()
    {
    }

    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Dt> Dts { get; set; }
    public DbSet<Unix> Unixs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("Idam.Libs.EF.Tests");
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.ChangeTracker.AddTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.ChangeTracker.AddTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddSoftDeleteFilter();

        base.OnModelCreating(modelBuilder);
    }
}
