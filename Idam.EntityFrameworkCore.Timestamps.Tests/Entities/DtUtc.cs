using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     The DateTime utc entity.
/// </summary>
/// <seealso cref="ITimeStampsUtc" />
/// <seealso cref="ISoftDeleteUtc" />
public class DtUtc : BaseEntity, ITimeStampsUtc, ISoftDeleteUtc
{
    [Precision(6)] public DateTime? DeletedAt { get; set; }

    [Precision(6)] public DateTime CreatedAt { get; set; }

    [Precision(6)] public DateTime UpdatedAt { get; set; }
}