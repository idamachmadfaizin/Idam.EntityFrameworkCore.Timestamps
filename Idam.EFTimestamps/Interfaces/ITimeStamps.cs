namespace Idam.EFTimestamps.Interfaces;

public interface ITimeStampBase;

/// <summary>
///     Timestamps local interface.
/// </summary>
public interface ITimeStamps : ICreatedAt, IUpdatedAt;

/// <summary>
///     Timestamps UTC interface.
/// </summary>
public interface ITimeStampsUtc : ICreatedAtUtc, IUpdatedAtUtc;

/// <summary>
///     Timestamps interface using Unix format
/// </summary>
public interface ITimeStampsUnix : ICreatedAtUnix, IUpdatedAtUnix;