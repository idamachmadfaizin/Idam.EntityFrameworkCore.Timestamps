using Idam.EFTimestamps.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace Idam.EFTimestamps.Extensions;

/// <summary>
/// DbContext extension class.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Add timestamps to the Entity with TimeStampsAttribute when state is Added or Modified or Deleted.
    /// </summary>
    /// <param name="changeTracker">The change tracker.</param>
    public static void AddTimestamps(this ChangeTracker changeTracker)
    {
        foreach (EntityEntry entityEntry in changeTracker.Entries())
        {
            entityEntry.AddTimestamps();
        }
    }

    /// <summary>
    /// Add timestamps to the Entity with TimeStampsAttribute when state is Added or Modified or Deleted.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    private static void AddTimestamps(this EntityEntry? entityEntry)
    {
        if (entityEntry is null) return;

        switch (entityEntry.State)
        {
            case EntityState.Added:
            case EntityState.Modified:
                UpdateTimeStamps(entityEntry.Entity, entityEntry.State);
                break;

            case EntityState.Deleted:
                if (entityEntry.Entity is ISoftDelete softDelete && softDelete.DeletedAt is null)
                {
                    entityEntry.State = EntityState.Modified;
                    softDelete.DeletedAt = DateTime.UtcNow;
                }
                else if (entityEntry.Entity is ISoftDeleteUnix softDeleteUnix && softDeleteUnix.DeletedAt is null)
                {
                    entityEntry.State = EntityState.Modified;
                    softDeleteUnix.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                }
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Updates the time stamps.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="entityState">State of the entity.</param>
    private static void UpdateTimeStamps(object entity, EntityState entityState)
    {
        if (entityState is not EntityState.Added and not EntityState.Modified) return;

        switch (entity)
        {
            case ITimeStamps timeStamps:
                var now = DateTime.UtcNow;

                timeStamps.UpdatedAt = now;

                if (entityState == EntityState.Added)
                {
                    timeStamps.CreatedAt = now;
                }
                break;

            case ITimeStampsUnix timeStampsUnix:
                var nowUnix = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                timeStampsUnix.UpdatedAt = nowUnix;

                if (entityState == EntityState.Added)
                {
                    timeStampsUnix.CreatedAt = nowUnix;
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Query Filter to get model where DeletedAt field is null.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void AddSoftDeleteFilter(this ModelBuilder builder)
    {
        var mutables = builder.Model.GetEntityTypes();

        foreach (var mutable in mutables)
        {
            builder.AddSoftDeleteFilter(mutable);
        }
    }

    /// <summary>
    /// Query Filter to get model where DeletedAt field is null.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="mutable">The mutable.</param>
    private static void AddSoftDeleteFilter(this ModelBuilder builder, IMutableEntityType? mutable)
    {
        if (mutable is null) return;

        if (!typeof(ISoftDelete).IsAssignableFrom(mutable.ClrType) &&
            !typeof(ISoftDeleteUnix).IsAssignableFrom(mutable.ClrType)) return;

        var propertyType = typeof(ISoftDelete).IsAssignableFrom(mutable.ClrType)
            ? typeof(DateTime?)
            : typeof(long?);

        var parameter = Expression.Parameter(mutable.ClrType, "e");
        //Type[] typeArguments = [typeof(ISoftDelete).IsAssignableFrom(mutable.ClrType) ? typeof(DateTime?) : typeof(long?)];

        //var body = Expression.Equal(
        //    Expression.Call(typeof(Microsoft.EntityFrameworkCore.EF), nameof(Microsoft.EntityFrameworkCore.EF.Property), typeArguments, parameter, Expression.Constant(nameof(ISoftDelete.DeletedAt))),
        //    Expression.Constant(null));

        var property = Expression.Property(parameter, nameof(ISoftDelete.DeletedAt));
        var body = Expression.Equal(property, Expression.Constant(null, propertyType));

        var expression = Expression.Lambda(body, parameter);

        builder.Entity(mutable.ClrType).HasQueryFilter(expression);
    }
}