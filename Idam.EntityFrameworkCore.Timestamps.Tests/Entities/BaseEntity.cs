using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     Base Entity
/// </summary>
public abstract class BaseEntity
{
    protected BaseEntity()
    {
    }

    [SetsRequiredMembers]
    protected BaseEntity(string name)
    {
        Name = name;
    }

    [Key] public int Id { get; init; }

    [StringLength(191)] public required string Name { get; set; }

    [StringLength(191)] public string? Description { get; init; }
}