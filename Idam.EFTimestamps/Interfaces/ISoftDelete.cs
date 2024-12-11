namespace Idam.EFTimestamps.Interfaces;

/// <summary>
///     SoftDelete base interface.
/// </summary>
public interface ISoftDeleteBase;

/// <summary>
///     SoftDelete interface using local DateTime format.
/// </summary>
public interface ISoftDelete : ISoftDeleteBase
{
    DateTime? DeletedAt { get; set; }
}

/// <summary>
///     SoftDelete interface using UTC DateTime format.
/// </summary>
public interface ISoftDeleteUtc : ISoftDeleteBase
{
    DateTime? DeletedAt { get; set; }
}

/// <summary>
///     SoftDelete interface using Unix format.
/// </summary>
public interface ISoftDeleteUnix : ISoftDeleteBase
{
    long? DeletedAt { get; set; }
}