using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     Root of a TPH hierarchy. EF Core only accepts a query filter on the root, so
///     AddSoftDeleteFilter must skip <see cref="Dog" />.
/// </summary>
public class Animal : BaseEntity, ISoftDelete
{
    [Precision(6)] public DateTime? DeletedAt { get; set; }
}

/// <summary>
///     Derived type in the TPH hierarchy.
/// </summary>
public class Dog : Animal
{
    public string? Breed { get; set; }
}
