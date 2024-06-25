namespace Idam.EFTimestamps.Interfaces;

/// <summary>
/// SoftDelete base
/// </summary>
public interface ISoftDeleteBase
{
}

/// <summary>
/// SoftDelete interface using DateTime format
/// </summary>
public interface ISoftDelete : ISoftDeleteBase
{
    DateTime? DeletedAt { get; set; }
}

/// <summary>
/// SoftDelete interface using Unix format
/// </summary>
public interface ISoftDeleteUnix : ISoftDeleteBase
{
    long? DeletedAt { get; set; }
}