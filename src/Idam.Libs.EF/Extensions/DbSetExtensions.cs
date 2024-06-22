using Idam.Libs.EF.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Idam.Libs.EF.Extensions;
public static class DbSetExtensions
{
    /// <summary>
    /// Restores the specified entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="dbSet">The database set.</param>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static TEntity Restore<TEntity>(this DbSet<TEntity> dbSet, TEntity entity)
        where TEntity : class, ISoftDeleteBase
    {
        ArgumentNullException.ThrowIfNull(dbSet, nameof(dbSet));

        if (entity is not ISoftDeleteBase) throw new ArgumentException($"The {nameof(entity)} must be {nameof(ISoftDelete)} or {nameof(ISoftDeleteUnix)}");

        switch (entity)
        {
            case ISoftDelete softDelete:
                softDelete.DeletedAt = null;
                break;
            case ISoftDeleteUnix softDeleteUnix:
                softDeleteUnix.DeletedAt = null;
                break;
            default:
                break;
        }

        return entity;
    }

    /// <summary>
    /// Forces the remove.
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

        if (entity is not ISoftDeleteBase) throw new ArgumentException($"The {nameof(entity)} must be {nameof(ISoftDelete)} or {nameof(ISoftDeleteUnix)}");

        switch (entity)
        {
            case ISoftDelete softDelete:
                softDelete.DeletedAt = DateTime.UtcNow;
                break;
            case ISoftDeleteUnix softDeleteUnix:
                softDeleteUnix.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                break;
            default:
                break;
        }

        return dbSet.Remove(entity);
    }
}
