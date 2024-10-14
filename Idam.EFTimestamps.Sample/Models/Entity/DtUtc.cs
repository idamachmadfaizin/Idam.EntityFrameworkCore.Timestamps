using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Idam.EFTimestamps.Interfaces;
using Idam.EFTimestamps.Sample.Models.Dto;

namespace Idam.EFTimestamps.Sample.Models.Entity;

/// <summary>The DateTime UTC entity.</summary>
/// <seealso cref="ITimeStampsUtc"/>
/// <seealso cref="ISoftDeleteUtc"/>
public class DtUtc : ITimeStampsUtc, ISoftDeleteUtc
{
    public DtUtc()
    {
    }
    
    [SetsRequiredMembers]
    public DtUtc(DtUtcCreateDto dto)
    {
        Name = dto.Name;
        Description = dto.Description;
    }
    
    [SetsRequiredMembers]
    public DtUtc(DtUtcUpdateDto dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Description = dto.Description;
    }

    [Key]
    public int Id { get; init; }

    [StringLength(191)]
    public required string Name { get; set; }

    [StringLength(191)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}