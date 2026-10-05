using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     The individual Offset interfaces without <see cref="ITimeStampsOffset" />, which sends the
///     entity through the <c>default</c> branch of UpdateTimeStamps.
/// </summary>
public class CreatedUpdatedOffset : BaseEntity, ICreatedAtOffset, IUpdatedAtOffset
{
    [Precision(6)] public DateTimeOffset CreatedAt { get; set; }

    [Precision(6)] public DateTimeOffset UpdatedAt { get; set; }
}
