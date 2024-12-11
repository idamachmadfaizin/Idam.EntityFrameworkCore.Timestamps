namespace Idam.EFTimestamps.Interfaces;

/// <summary>
///     UpdatedAt interface using local DateTime format.
/// </summary>
public interface IUpdatedAt : ITimeStampBase
{
    DateTime UpdatedAt { get; set; }
}

/// <summary>
///     UpdatedAt interface using UTC DateTime format.
/// </summary>
public interface IUpdatedAtUtc : ITimeStampBase
{
    DateTime UpdatedAt { get; set; }
}

/// <summary>
///     UpdatedAt interface using Unix format.
/// </summary>
public interface IUpdatedAtUnix : ITimeStampBase
{
    long UpdatedAt { get; set; }
}