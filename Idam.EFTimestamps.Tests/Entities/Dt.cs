using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

/// <summary>
///     The DateTime entity.
/// </summary>
/// <seealso cref="ITimeStamps" />
/// <seealso cref="ISoftDelete" />
public class Dt : BaseEntity, ITimeStamps, ISoftDelete
{
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}