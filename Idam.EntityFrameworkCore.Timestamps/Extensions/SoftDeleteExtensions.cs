using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

public static class SoftDeleteExtensions
{
    extension(ISoftDeleteBase entity)
    {
        /// <summary>
        ///     Determines whether this instance is deleted.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the specified entity is deleted; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Trashed()
        {
            ArgumentNullException.ThrowIfNull(entity);

            var entityType = entity.GetType();
            var deletedAtProperty = entityType.GetProperty(nameof(ISoftDelete.DeletedAt));

            ArgumentNullException.ThrowIfNull(deletedAtProperty);

            var value = deletedAtProperty.GetValue(entity);

            return value is not null;
        }
    }
}