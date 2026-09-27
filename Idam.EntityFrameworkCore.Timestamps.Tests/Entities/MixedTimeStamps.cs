using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     Local CreatedAt combined with UTC UpdatedAt. This is the only shape that reaches the
///     <c>default</c> branch of UpdateTimeStamps, where CreatedAt and UpdatedAt are handled
///     by separate switches.
/// </summary>
public class MixedTimeStamps : BaseEntity, ICreatedAt, IUpdatedAtUtc
{
    [Precision(6)] public DateTime CreatedAt { get; set; }

    [Precision(6)] public DateTime UpdatedAt { get; set; }
}
