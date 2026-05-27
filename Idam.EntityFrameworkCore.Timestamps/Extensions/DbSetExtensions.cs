using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public EntityEntry<TEntity> ForceRemove(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(dbSet);

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
}