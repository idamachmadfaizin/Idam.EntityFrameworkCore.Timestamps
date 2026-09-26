using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

/// <summary>
///     DbContext extension class.
/// </summary>
public static class DbContextExtensions
{
    extension(ChangeTracker changeTracker)
    {
        /// <summary>
        ///     Add timestamps to the Entity with TimeStampsAttribute when state is Added or Modified or Deleted.
        /// </summary>
        public void AddTimestamps()
        {
            changeTracker.DetectChanges();

            var timestamp = DateTimeOffset.Now;

            foreach (var entityEntry in changeTracker.Entries().ToList()) entityEntry.AddTimestamps(timestamp);
        }
    }

    extension(EntityEntry? entityEntry)
    {
        /// <summary>
        ///     Add timestamps to the Entity with TimeStampsAttribute when state is Added or Modified or Deleted.
        /// </summary>
        /// <param name="timestamp">The timestamp shared by every entity in the current save.</param>
        private void AddTimestamps(DateTimeOffset timestamp)
        {
            if (entityEntry is null) return;

            switch (entityEntry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    UpdateTimeStamps(entityEntry, timestamp);
                    break;

                case EntityState.Deleted:
                    UpdateSoftDelete(entityEntry, timestamp);
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    break;
            }
        }
    }

    /// <summary>
    ///     Updates the time stamps.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    /// <param name="timestamp">The timestamp shared by every entity in the current save.</param>
    private static void UpdateTimeStamps(EntityEntry entityEntry, DateTimeOffset timestamp)
    {
        if (entityEntry.State is not EntityState.Added and not EntityState.Modified) return;
        if (entityEntry.Entity is not ITimeStampBase) return;

        var now = timestamp.LocalDateTime;
        var nowUtc = timestamp.UtcDateTime;
        var nowUnix = timestamp.ToUnixTimeMilliseconds();

        switch (entityEntry.Entity)
        {
            case ITimeStamps timeStamps:
                timeStamps.UpdatedAt = now;
                if (entityEntry.State == EntityState.Added) timeStamps.CreatedAt = now;

                break;

            case ITimeStampsUtc timeStampsUtc:
                timeStampsUtc.UpdatedAt = nowUtc;
                if (entityEntry.State == EntityState.Added) timeStampsUtc.CreatedAt = nowUtc;

                break;

            case ITimeStampsUnix timeStampsUnix:
                timeStampsUnix.UpdatedAt = nowUnix;
                if (entityEntry.State == EntityState.Added) timeStampsUnix.CreatedAt = nowUnix;

                break;

            default:
                if (entityEntry.State == EntityState.Added)
                {
                    switch (entityEntry.Entity)
                    {
                        case ICreatedAt createdAt:
                            createdAt.CreatedAt = now;
                            break;
                        case ICreatedAtUtc createdAtUtc:
                            createdAtUtc.CreatedAt = nowUtc;
                            break;
                        case ICreatedAtUnix createdAtUnix:
                            createdAtUnix.CreatedAt = nowUnix;
                            break;
                    }
                }

                switch (entityEntry.Entity)
                {
                    case IUpdatedAt updatedAt:
                        updatedAt.UpdatedAt = now;
                        break;
                    case IUpdatedAtUtc updatedAtUtc:
                        updatedAtUtc.UpdatedAt = nowUtc;
                        break;
                    case IUpdatedAtUnix updatedAtUnix:
                        updatedAtUnix.UpdatedAt = nowUnix;
                        break;
                }

                break;
        }
    }

    /// <summary>
    ///     Updates the soft delete.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    /// <param name="timestamp">The timestamp shared by every entity in the current save.</param>
    private static void UpdateSoftDelete(EntityEntry entityEntry, DateTimeOffset timestamp)
    {
        if (entityEntry.State is not EntityState.Deleted) return;
        if (entityEntry.Entity is not ISoftDeleteBase) return;

        switch (entityEntry.Entity)
        {
            case ISoftDelete { DeletedAt: null } softDelete:
                entityEntry.State = EntityState.Modified;
                softDelete.DeletedAt = timestamp.LocalDateTime;
                break;
            case ISoftDeleteUtc { DeletedAt: null } softDeleteUtc:
                entityEntry.State = EntityState.Modified;
                softDeleteUtc.DeletedAt = timestamp.UtcDateTime;
                break;
            case ISoftDeleteUnix { DeletedAt: null } softDeleteUnix:
                entityEntry.State = EntityState.Modified;
                softDeleteUnix.DeletedAt = timestamp.ToUnixTimeMilliseconds();
                break;
        }
    }
}