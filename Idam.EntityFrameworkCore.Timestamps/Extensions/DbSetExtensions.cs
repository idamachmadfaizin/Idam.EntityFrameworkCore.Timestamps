using Idam.EntityFrameworkCore.Timestamps.Interceptors;
using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

public static class DbSetExtensions
{
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

            SoftDeleteFormats.Restore(entity);

            // Update() attaches the entity when it is not tracked yet; without it a detached
            // or no-tracking entity is cleared in memory only and SaveChanges() writes nothing.
            return dbSet.Update(entity).Entity;
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

            var context = dbSet.GetService<ICurrentDbContext>().Context;
            SoftDeleteFormats.TryStamp(entity, Timestamp.Now(TimeStampsInterceptor.ResolveTimeProvider(context)));

            return dbSet.Remove(entity);
        }
    }
}