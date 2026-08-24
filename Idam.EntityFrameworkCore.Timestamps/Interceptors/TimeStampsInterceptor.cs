using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Idam.EntityFrameworkCore.Timestamps.Interceptors;

public class TimeStampsInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        eventData.Context?.ChangeTracker.AddTimestamps();
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        eventData.Context?.ChangeTracker.AddTimestamps();
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}