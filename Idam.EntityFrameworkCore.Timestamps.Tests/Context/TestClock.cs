namespace Idam.EntityFrameworkCore.Timestamps.Tests.Context;

/// <summary>
///     A clock that stands still unless a test moves it, so every timestamp is known up front.
/// </summary>
/// <param name="start">The instant the clock starts at.</param>
public sealed class TestClock(DateTimeOffset start) : TimeProvider
{
    private DateTimeOffset _now = start;

    /// <summary>
    ///     Added after every read. A non-zero step proves a save reads the clock only once:
    ///     reading it per entity would hand each entity a different value.
    /// </summary>
    public TimeSpan Step { get; set; }

    /// <summary>
    ///     Overrides the zone the local-time interfaces convert to; the machine's zone when null.
    /// </summary>
    public TimeZoneInfo? LocalZone { get; init; }

    public override TimeZoneInfo LocalTimeZone => LocalZone ?? base.LocalTimeZone;

    public override DateTimeOffset GetUtcNow()
    {
        var now = _now;
        _now += Step;
        return now;
    }

    public void Advance(TimeSpan by)
    {
        _now += by;
    }
}
