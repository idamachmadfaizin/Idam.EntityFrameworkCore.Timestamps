using Idam.EntityFrameworkCore.Timestamps.Constants;
using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

public static class ModelBuilderExtensions
{
    /// <param name="builder">The builder.</param>
    extension(ModelBuilder builder)
    {
        /// <summary>
        ///     Query Filter to get model where DeletedAt field is null.
        /// </summary>
        public void AddSoftDeleteFilter()
        {
            var mutableEntityTypes = builder.Model.GetEntityTypes();

            foreach (var mutableEntityType in mutableEntityTypes) builder.AddSoftDeleteFilter(mutableEntityType);
        }

        /// <summary>
        ///     Query Filter to get model where DeletedAt field is null.
        /// </summary>
        /// <param name="mutable">The mutable entity type.</param>
        private void AddSoftDeleteFilter(IMutableEntityType? mutable)
        {
            if (mutable is null) return;

            if (!typeof(ISoftDelete).IsAssignableFrom(mutable.ClrType) &&
                !typeof(ISoftDeleteUtc).IsAssignableFrom(mutable.ClrType) &&
                !typeof(ISoftDeleteUnix).IsAssignableFrom(mutable.ClrType)) return;

            var propertyType = typeof(ISoftDelete).IsAssignableFrom(mutable.ClrType)
                ? typeof(DateTime?)
                : typeof(long?);

            var parameter = Expression.Parameter(mutable.ClrType, "e");

            var property = Expression.Property(parameter, nameof(ISoftDelete.DeletedAt));
            var body = Expression.Equal(property, Expression.Constant(null, propertyType));

            var expression = Expression.Lambda(body, parameter);

            builder.Entity(mutable.ClrType).HasQueryFilter(SoftDeleteFilters.Default, expression);
        }
    }
}
