using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

/// <summary>
///     The Unix entity
/// </summary>
/// <seealso cref="ITimeStampsUnix" />
/// <seealso cref="ISoftDeleteUnix" />
public class Unix : BaseEntity, ITimeStampsUnix, ISoftDeleteUnix
{
    public long? DeletedAt { get; set; }
    public long CreatedAt { get; set; }

    public long UpdatedAt { get; set; }
}