namespace Idam.EFTimestamps.Interfaces;

/// <summary>
/// Timestamps interface using Unix format
/// </summary>
public interface ITimeStamps
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Timestamps interface using Unix format
/// </summary>
public interface ITimeStampsUnix
{
    long CreatedAt { get; set; }
    long UpdatedAt { get; set; }
}
