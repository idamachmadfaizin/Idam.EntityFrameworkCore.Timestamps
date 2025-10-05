using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     The DateTime utc entity.
/// </summary>
/// <seealso cref="ITimeStampsUtc" />
/// <seealso cref="ISoftDeleteUtc" />
public class DtUtc : BaseEntity, ITimeStampsUtc, ISoftDeleteUtc
{
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}