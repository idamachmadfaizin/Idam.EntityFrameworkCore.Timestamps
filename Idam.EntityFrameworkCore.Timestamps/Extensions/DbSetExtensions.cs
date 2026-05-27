using System.Linq.Expressions;
using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

public static class DbSetExtensions
{
    extension<TEntity>(IQueryable<TEntity> query) where TEntity : class, ISoftDeleteBase
    {
        /// <summary>
        ///     Filters entities where DeletedAt is null.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IQueryable<TEntity> WhereActive()
        {
            ArgumentNullException.ThrowIfNull(query);

            return query.Where(BuildDeletedAtExpression<TEntity>(false));
        }

        /// <summary>
        ///     Filters entities where DeletedAt is not null.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IQueryable<TEntity> WhereTrashed()
        {
            ArgumentNullException.ThrowIfNull(query);

            return query.Where(BuildDeletedAtExpression<TEntity>(true));
        }
    }

    /// <param name="dbSet">The database set.</param>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    extension<TEntity>(DbSet<TEntity> dbSet) where TEntity : class, ISoftDeleteBase
    {
        /// <summary>
        ///     Restores the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public TEntity Restore(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(dbSet);
            ArgumentNullException.ThrowIfNull(entity);
            ArgumentNullException.ThrowIfNull(entity);

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
        ///     Restores the specified entity and persists immediately.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> RestoreAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dbSet);
            ArgumentNullException.ThrowIfNull(entity);

            var context = dbSet.GetService<ICurrentDbContext>().Context;

            var entry = context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                dbSet.Attach(entity);
                entry = context.Entry(entity);
            }

            dbSet.Restore(entity);
            entry.Property(nameof(ISoftDelete.DeletedAt)).IsModified = true;

            return await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        ///     Soft deletes the specified entity and persists immediately.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<int> SoftDeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(dbSet);
            ArgumentNullException.ThrowIfNull(entity);

            dbSet.Remove(entity);

            var context = dbSet.GetService<ICurrentDbContext>().Context;
            return await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        ///     Forces the remove.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public EntityEntry<TEntity> ForceRemove(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(dbSet);
            ArgumentNullException.ThrowIfNull(entity);

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

    /// <summary>
    ///     Builds a soft-delete predicate for the DeletedAt property.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="deleted"></param>
    /// <returns></returns>
    private static Expression<Func<TEntity, bool>> BuildDeletedAtExpression<TEntity>(bool deleted)
        where TEntity : class, ISoftDeleteBase
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var deletedAt = Expression.Property(parameter, nameof(ISoftDelete.DeletedAt));
        var nullConstant = Expression.Constant(null, deletedAt.Type);

        var body = deleted
            ? Expression.NotEqual(deletedAt, nullConstant)
            : Expression.Equal(deletedAt, nullConstant);

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}

