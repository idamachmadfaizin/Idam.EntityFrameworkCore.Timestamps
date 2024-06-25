using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

/// <summary>
/// The DateTime entity.
/// </summary>
/// <seealso cref="ITimeStamps" />
/// <seealso cref="ISoftDelete" />
public class Dt : BaseEntity, ITimeStamps, ISoftDelete
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
