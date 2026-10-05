namespace Idam.EntityFrameworkCore.Timestamps.Interfaces;

/// <summary>
///     CreatedAt interface using local DateTime format.
/// </summary>
public interface ICreatedAt : ITimeStampBase
{
    DateTime CreatedAt { get; set; }
}

/// <summary>
///     CreatedAt interface using UTC DateTime format.
/// </summary>
public interface ICreatedAtUtc : ITimeStampBase
{
    DateTime CreatedAt { get; set; }
}

/// <summary>
///     CreatedAt interface using Unix format.
/// </summary>
public interface ICreatedAtUnix : ITimeStampBase
{
    long CreatedAt { get; set; }
}

/// <summary>
///     CreatedAt interface using DateTimeOffset format, always written with offset zero (UTC).
/// </summary>
public interface ICreatedAtOffset : ITimeStampBase
{
    DateTimeOffset CreatedAt { get; set; }
}
