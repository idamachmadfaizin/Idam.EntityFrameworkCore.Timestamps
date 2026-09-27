using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Idam.EntityFrameworkCore.Timestamps.Sample.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Sample.Context;

public class MyDbContext(DbContextOptions options, IConfiguration configuration) : DbContext(options)
{
    public DbSet<Unix> Unixs { get; init; }
    public DbSet<Dt> Dts { get; init; }
    public DbSet<DtUtc> DtUtcs { get; init; }
    public DbSet<DtCustom> DtCustoms { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        optionsBuilder.AddTimeStampsInterceptor();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddSoftDeleteFilter();

        base.OnModelCreating(modelBuilder);
    }
}