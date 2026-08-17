using Idam.EntityFrameworkCore.Timestamps.Constants;
using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

public static class IQueryableExtensions
{
    extension<TEntity>(IQueryable<TEntity> query) where TEntity : class, ISoftDeleteBase
    {
        /// <summary>
        /// Includes soft-deleted entities in the query results
        /// </summary>
        /// <returns>
        /// A query that does not apply the default soft-delete query filter.
        /// </returns>
        public IQueryable<TEntity> IncludeTrashed()
        {
            return query.IgnoreQueryFilters([SoftDeleteFilters.Default]);
        }

        /// <summary>
        /// Returns only soft-deleted entities in the query results.
        /// </summary>
        /// <returns>
        /// A query containing only entities that have been soft-deleted.
        /// </returns>
        public IQueryable<TEntity> OnlyTrashed()
        {
            return query.IncludeTrashed().Where(x => x.Trashed());
        }
    }
}