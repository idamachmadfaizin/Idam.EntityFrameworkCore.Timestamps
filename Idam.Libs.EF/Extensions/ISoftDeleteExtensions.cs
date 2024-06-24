using Idam.Libs.EF.Interfaces;

namespace Idam.Libs.EF.Extensions;
public static class ISoftDeleteExtensions
{
    /// <summary>
    /// Determines whether this instance is deleted.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>
    ///   <c>true</c> if the specified entity is deleted; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool Trashed(this ISoftDeleteBase entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        Type entityType = entity.GetType();

        var deletedAtProperty = entityType.GetProperty(nameof(ISoftDelete.DeletedAt));

        ArgumentNullException.ThrowIfNull(deletedAtProperty, nameof(deletedAtProperty));

        var value = deletedAtProperty.GetValue(entity);

        return value is not null;
    }
}
