using Idam.EFTimestamps.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Idam.EFTimestamps.Extensions;

public static class DbSetExtensions
{
    /// <summary>
    ///     Restores the specified entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="dbSet">The database set.</param>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static TEntity Restore<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class, ISoftDeleteBase
    {
        ArgumentNullException.ThrowIfNull(dbSet, nameof(dbSet));

        switch (entity)
        {
            case ISoftDelete softDelete:
                softDelete.DeletedAt = null;
                break;
            case ISoftDeleteUtc softDeleteUtc:
                softDeleteUtc.DeletedAt = null;
                break;
            case ISoftDeleteUnix softDeleteUnix:
                softDeleteUnix.DeletedAt = null;
                break;
        }

        return entity;
    }

    /// <summary>
    ///     Forces the remove.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="dbSet">The database set.</param>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static EntityEntry<TEntity> ForceRemove<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class, ISoftDeleteBase
    {
        ArgumentNullException.ThrowIfNull(dbSet, nameof(dbSet));

        switch (entity)
        {
            case ISoftDelete softDelete:
                softDelete.DeletedAt = DateTime.Now;
                break;
            case ISoftDeleteUtc softDeleteUtc:
                softDeleteUtc.DeletedAt = DateTime.UtcNow;
                break;
            case ISoftDeleteUnix softDeleteUnix:
                softDeleteUnix.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                break;
        }

        return dbSet.Remove(entity);
    }
}