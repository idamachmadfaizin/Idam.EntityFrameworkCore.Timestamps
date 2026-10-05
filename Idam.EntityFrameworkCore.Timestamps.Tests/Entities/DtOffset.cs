using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     The DateTimeOffset entity.
/// </summary>
/// <seealso cref="ITimeStampsOffset" />
/// <seealso cref="ISoftDeleteOffset" />
public class DtOffset : BaseEntity, ITimeStampsOffset, ISoftDeleteOffset
{
    [Precision(6)] public DateTimeOffset? DeletedAt { get; set; }

    [Precision(6)] public DateTimeOffset CreatedAt { get; set; }

    [Precision(6)] public DateTimeOffset UpdatedAt { get; set; }
}
