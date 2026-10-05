using Idam.EntityFrameworkCore.Timestamps.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Idam.EntityFrameworkCore.Timestamps.Interceptors;

public class TimeStampsInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider? _timeProvider;

    /// <summary>
    ///     Reads the time from the context's clock: a <see cref="TimeStampsInterceptor" /> registered
    ///     with a <see cref="TimeProvider" />, else the application's <see cref="TimeProvider" />
    ///     service, else the system clock.
    /// </summary>
    public TimeStampsInterceptor()
    {
    }

    /// <param name="timeProvider">The clock the timestamps are read from.</param>
    public TimeStampsInterceptor(TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);
        _timeProvider = timeProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AddTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddTimestamps(DbContext? context)
    {
        context?.ChangeTracker.AddTimestamps(_timeProvider ?? ResolveTimeProvider(context));
    }

    /// <summary>
    ///     The clock a context's timestamps come from. An explicitly configured one wins, so a second,
    ///     parameterless registration, or the <c>SaveChanges</c> overrides, cannot stamp with another clock.
    /// </summary>
    internal static TimeProvider ResolveTimeProvider(DbContext context)
    {
        var options = context.GetService<IDbContextOptions>().FindExtension<CoreOptionsExtension>();

        return options?.Interceptors?.OfType<TimeStampsInterceptor>().LastOrDefault(i => i._timeProvider is not null)?._timeProvider
            ?? options?.ApplicationServiceProvider?.GetService<TimeProvider>()
            ?? TimeProvider.System;
    }
}
