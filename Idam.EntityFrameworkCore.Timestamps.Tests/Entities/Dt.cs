using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     The DateTime entity.
/// </summary>
/// <seealso cref="ITimeStamps" />
/// <seealso cref="ISoftDelete" />
public class Dt : BaseEntity, ITimeStamps, ISoftDelete
{
    [Precision(6)] public DateTime? DeletedAt { get; set; }

    [Precision(6)] public DateTime CreatedAt { get; set; }

    [Precision(6)] public DateTime UpdatedAt { get; set; }
}