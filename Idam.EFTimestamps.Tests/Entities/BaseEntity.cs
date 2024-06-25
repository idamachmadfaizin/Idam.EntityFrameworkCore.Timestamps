using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Idam.EFTimestamps.Tests.Entities;
/// <summary>
/// Base Entity
/// </summary>
/// <seealso cref="IGuidEntity" />
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

    [Key]
    public int Id { get; set; }

    [StringLength(191)]
    public required string Name { get; set; }

    [StringLength(191)]
    public string? Description { get; set; }
}
