using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

/// <summary>
///     The only place that knows each soft-delete format. <c>OnlyTrashed()</c> and the global query
///     filter do not depend on the format, so a new one needs a case here and nowhere else.
/// </summary>
internal static class SoftDeleteFormats
{
    public static bool IsTrashed(ISoftDeleteBase entity)
    {
        return entity switch
        {
            ISoftDelete { DeletedAt: not null } => true,
            ISoftDeleteUtc { DeletedAt: not null } => true,
            ISoftDeleteOffset { DeletedAt: not null } => true,
            ISoftDeleteUnix { DeletedAt: not null } => true,
            _ => false
        };
    }

    public static void Restore(ISoftDeleteBase entity)
    {
        switch (entity)
        {
            case ISoftDelete softDelete:
                softDelete.DeletedAt = null;
                break;
            case ISoftDeleteUtc softDeleteUtc:
                softDeleteUtc.DeletedAt = null;
                break;
            case ISoftDeleteOffset softDeleteOffset:
                softDeleteOffset.DeletedAt = null;
                break;
            case ISoftDeleteUnix softDeleteUnix:
                softDeleteUnix.DeletedAt = null;
                break;
        }
    }

    /// <summary>
    ///     Stamps <c>DeletedAt</c> when it is not set yet.
    /// </summary>
    /// <returns><c>false</c> when the entity is already trashed or has no <c>DeletedAt</c>.</returns>
    public static bool TryStamp(object entity, Timestamp now)
    {
        switch (entity)
        {
            case ISoftDelete { DeletedAt: null } softDelete:
                softDelete.DeletedAt = now.Local;
                return true;
            case ISoftDeleteUtc { DeletedAt: null } softDeleteUtc:
                softDeleteUtc.DeletedAt = now.Utc;
                return true;
            case ISoftDeleteOffset { DeletedAt: null } softDeleteOffset:
                softDeleteOffset.DeletedAt = now.Offset;
                return true;
            case ISoftDeleteUnix { DeletedAt: null } softDeleteUnix:
                softDeleteUnix.DeletedAt = now.Unix;
                return true;
            default:
                return false;
        }
    }
}
