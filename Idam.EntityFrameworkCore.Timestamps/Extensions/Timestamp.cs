namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

/// <summary>
///     One instant in every format the interfaces store, read from the clock once per save so that
///     every entity in it gets the same value.
/// </summary>
internal readonly record struct Timestamp(DateTime Local, DateTime Utc, long Unix, DateTimeOffset Offset)
{
    public static Timestamp Now(TimeProvider timeProvider)
    {
        // GetUtcNow() should already be UTC. Normalising it keeps the offset-zero guarantee, which
        // PostgreSQL enforces, even for a TimeProvider that returns another offset.
        var utc = timeProvider.GetUtcNow().ToUniversalTime();
        var zone = timeProvider.LocalTimeZone;

        // Kind=Local means the machine's zone, so any other zone comes back Unspecified.
        var local = zone.Equals(TimeZoneInfo.Local)
            ? utc.LocalDateTime
            : TimeZoneInfo.ConvertTime(utc, zone).DateTime;

        return new Timestamp(local, utc.UtcDateTime, utc.ToUnixTimeMilliseconds(), utc);
    }
}
