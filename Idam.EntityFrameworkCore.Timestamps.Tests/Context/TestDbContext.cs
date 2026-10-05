using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Idam.EntityFrameworkCore.Timestamps.Tests.Entities;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Context;

public class TestDbContext(DbContextOptions<TestDbContext> options, TimeProvider? clock) : DbContext(options)
{
    public DbSet<Dt> Dts { get; init; }
    public DbSet<DtUtc> DtUtcs { get; init; }
    public DbSet<DtOffset> DtOffsets { get; init; }
    public DbSet<CreatedUpdatedOffset> CreatedUpdatedOffsets { get; init; }
    public DbSet<Unix> Unixs { get; init; }
    public DbSet<CreatedAtEntity> CreatedAts { get; init; }
    public DbSet<CreatedAtUnixEntity> CreatedAtUnixs { get; init; }
    public DbSet<CreatedAtUtcEntity> CreatedAtUtcs { get; init; }
    public DbSet<UpdatedAtEntity> UpdatedAts { get; init; }
    public DbSet<UpdatedAtUnixEntity> UpdatedAtUnixs { get; init; }
    public DbSet<UpdatedAtUtcEntity> UpdatedAtUtcs { get; init; }
    public DbSet<SoftDeleteOnly> SoftDeleteOnlys { get; init; }
    public DbSet<MixedTimeStamps> MixedTimeStamps { get; init; }
    public DbSet<RenamedColumns> RenamedColumns { get; init; }
    public DbSet<Animal> Animals { get; init; }
    public DbSet<Dog> Dogs { get; init; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Without a clock: the parameterless registration most applications use.
        if (clock is null) optionsBuilder.AddTimeStampsInterceptor();
        else optionsBuilder.AddTimeStampsInterceptor(clock);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Animal/Dog is a TPH hierarchy: the soft-delete filter must land on the root only.
        modelBuilder.Entity<Animal>();
        modelBuilder.Entity<Dog>();

        // A second named filter, so tests can prove IncludeTrashed() leaves other filters alone.
        modelBuilder.Entity<Dt>().HasQueryFilter(TenantFilter, e => e.Description != HiddenTenant);

        MapLocalDateTimesForNpgsql(modelBuilder);

        modelBuilder.AddSoftDeleteFilter();

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    ///     Npgsql maps <see cref="DateTime" /> to <c>timestamp with time zone</c>, which refuses any
    ///     value whose Kind is Local. The local-time interfaces therefore cannot be used on
    ///     PostgreSQL without pinning the column to <c>timestamp without time zone</c>.
    /// </summary>
    private void MapLocalDateTimesForNpgsql(ModelBuilder modelBuilder)
    {
        if (!Database.IsNpgsql()) return;

        (Type Interface, string Property)[] localMembers =
        [
            (typeof(ICreatedAt), nameof(ICreatedAt.CreatedAt)),
            (typeof(IUpdatedAt), nameof(IUpdatedAt.UpdatedAt)),
            (typeof(ISoftDelete), nameof(ISoftDelete.DeletedAt))
        ];

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Derived types share the root's properties, so configuring them again would fail.
            if (entityType.BaseType is not null) continue;

            foreach (var (@interface, property) in localMembers)
            {
                if (!@interface.IsAssignableFrom(entityType.ClrType)) continue;

                modelBuilder.Entity(entityType.ClrType)
                    .Property(property)
                    .HasColumnType("timestamp without time zone");
            }
        }
    }

    public const string TenantFilter = "Tests.Tenant";
    public const string HiddenTenant = "other-tenant";
}
