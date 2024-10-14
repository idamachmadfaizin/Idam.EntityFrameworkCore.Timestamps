namespace Idam.EFTimestamps.Tests.Ekstensions;
internal static class DateTimeExtension
{
    /// <summary>Converts to Unix time milliseconds.</summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>
    ///   <br />
    /// </returns>
    public static long ToUnixTimeMilliseconds(this DateTime dateTime) =>
        new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
    
    /// <summary>
    /// Check is a DateTime is in UTC.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static bool IsUtc(this DateTime dateTime) => dateTime.Kind == DateTimeKind.Utc;
}
