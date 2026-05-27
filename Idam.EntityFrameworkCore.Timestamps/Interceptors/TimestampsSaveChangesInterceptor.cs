using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Idam.EntityFrameworkCore.Timestamps.Interceptors;

/// <summary>
///     Applies timestamp and soft-delete updates during SaveChanges.
/// </summary>
public sealed class TimestampsSaveChangesInterceptor : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        eventData.Context?.ChangeTracker.AddTimestamps();

        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        eventData.Context?.ChangeTracker.AddTimestamps();

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
