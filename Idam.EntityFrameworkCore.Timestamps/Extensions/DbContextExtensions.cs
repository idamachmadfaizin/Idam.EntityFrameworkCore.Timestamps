using Idam.EntityFrameworkCore.Timestamps.Interceptors;
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
        ///     Add timestamps to the Entity with TimeStampsAttribute when state is Added or Modified or Deleted,
        ///     reading the context's clock: a TimeStampsInterceptor registered with a <see cref="TimeProvider" />,
        ///     else the application's <see cref="TimeProvider" /> service, else the system clock.
        /// </summary>
        public void AddTimestamps()
        {
            changeTracker.AddTimestamps(TimeStampsInterceptor.ResolveTimeProvider(changeTracker.Context));
        }

        /// <summary>
        ///     Add timestamps to the Entity with TimeStampsAttribute when state is Added or Modified or Deleted.
        /// </summary>
        /// <param name="timeProvider">
        ///     The clock the timestamps are read from. Local-time interfaces convert with its
        ///     <see cref="TimeProvider.LocalTimeZone" />: Kind is Local when that is the machine's zone, and
        ///     Unspecified otherwise.
        /// </param>
        public void AddTimestamps(TimeProvider timeProvider)
        {
            ArgumentNullException.ThrowIfNull(timeProvider);

            var now = Timestamp.Now(timeProvider);

            var entries = changeTracker.Entries()
                .Where(entry => entry.State is EntityState.Added
                    or EntityState.Modified
                    or EntityState.Deleted)
                .ToList();

            foreach (var entityEntry in entries) entityEntry.AddTimestamps(now);
        }
    }

    extension(EntityEntry? entityEntry)
    {
        /// <summary>
        ///     Add timestamps to the Entity with TimeStampsAttribute when state is Added or Modified or Deleted.
        /// </summary>
        /// <param name="now">The timestamp shared by every entity in the current save.</param>
        private void AddTimestamps(Timestamp now)
        {
            if (entityEntry is null) return;

            switch (entityEntry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    UpdateTimeStamps(entityEntry, now);
                    break;

                case EntityState.Deleted:
                    UpdateSoftDelete(entityEntry, now);
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
    /// <param name="now">The timestamp shared by every entity in the current save.</param>
    private static void UpdateTimeStamps(EntityEntry entityEntry, Timestamp now)
    {
        if (entityEntry.State is not EntityState.Added and not EntityState.Modified) return;
        if (entityEntry.Entity is not ITimeStampBase) return;

        switch (entityEntry.Entity)
        {
            case ITimeStamps timeStamps:
                timeStamps.UpdatedAt = now.Local;
                if (entityEntry.State == EntityState.Added) timeStamps.CreatedAt = now.Local;

                break;

            case ITimeStampsUtc timeStampsUtc:
                timeStampsUtc.UpdatedAt = now.Utc;
                if (entityEntry.State == EntityState.Added) timeStampsUtc.CreatedAt = now.Utc;

                break;

            case ITimeStampsUnix timeStampsUnix:
                timeStampsUnix.UpdatedAt = now.Unix;
                if (entityEntry.State == EntityState.Added) timeStampsUnix.CreatedAt = now.Unix;

                break;

            case ITimeStampsOffset timeStampsOffset:
                timeStampsOffset.UpdatedAt = now.Offset;
                if (entityEntry.State == EntityState.Added) timeStampsOffset.CreatedAt = now.Offset;

                break;

            default:
                if (entityEntry.State == EntityState.Added)
                {
                    switch (entityEntry.Entity)
                    {
                        case ICreatedAt createdAt:
                            createdAt.CreatedAt = now.Local;
                            break;
                        case ICreatedAtUtc createdAtUtc:
                            createdAtUtc.CreatedAt = now.Utc;
                            break;
                        case ICreatedAtUnix createdAtUnix:
                            createdAtUnix.CreatedAt = now.Unix;
                            break;
                        case ICreatedAtOffset createdAtOffset:
                            createdAtOffset.CreatedAt = now.Offset;
                            break;
                    }
                }

                switch (entityEntry.Entity)
                {
                    case IUpdatedAt updatedAt:
                        updatedAt.UpdatedAt = now.Local;
                        break;
                    case IUpdatedAtUtc updatedAtUtc:
                        updatedAtUtc.UpdatedAt = now.Utc;
                        break;
                    case IUpdatedAtUnix updatedAtUnix:
                        updatedAtUnix.UpdatedAt = now.Unix;
                        break;
                    case IUpdatedAtOffset updatedAtOffset:
                        updatedAtOffset.UpdatedAt = now.Offset;
                        break;
                }

                break;
        }
    }

    /// <summary>
    ///     Updates the soft delete.
    /// </summary>
    /// <param name="entityEntry">The entity entry.</param>
    /// <param name="now">The timestamp shared by every entity in the current save.</param>
    private static void UpdateSoftDelete(EntityEntry entityEntry, Timestamp now)
    {
        if (entityEntry.State is not EntityState.Deleted) return;
        if (SoftDeleteFormats.TryStamp(entityEntry.Entity, now)) entityEntry.State = EntityState.Modified;
    }
}