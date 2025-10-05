using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Idam.EntityFrameworkCore.Timestamps.Sample.Models.Dto;

namespace Idam.EntityFrameworkCore.Timestamps.Sample.Models.Entity;

/// <summary>
///     The DateTime entity.
/// </summary>
/// <seealso cref="ITimeStamps" />
/// <seealso cref="ISoftDelete" />
public class Dt : ITimeStamps, ISoftDelete
{
    public Dt()
    {
    }

    [SetsRequiredMembers]
    public Dt(DtCreateDto dto)
    {
        Name = dto.Name;
        Description = dto.Description;
    }

    [SetsRequiredMembers]
    public Dt(DtUpdateDto dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Description = dto.Description;
    }

    [Key] public int Id { get; init; }

    [StringLength(191)] public required string Name { get; set; }

    [StringLength(191)] public string? Description { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}