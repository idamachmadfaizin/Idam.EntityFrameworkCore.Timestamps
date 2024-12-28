using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Idam.EFTimestamps.Interfaces;
using Idam.EFTimestamps.Sample.Models.Dto;

namespace Idam.EFTimestamps.Sample.Models.Entity;

public class DtCustom : ITimeStamps, ISoftDelete
{
    public DtCustom()
    {
    }

    [SetsRequiredMembers]
    public DtCustom(DtCustomCreatedDto dto)
    {
        Name = dto.Name;
        Description = dto.Description;
    }

    [SetsRequiredMembers]
    public DtCustom(DtCustomUpdatedDto dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Description = dto.Description;
    }
    
    [Key] public int Id { get; init; }

    [StringLength(191)] public required string Name { get; set; }

    [StringLength(191)] public string? Description { get; set; }
    
    [Column("AddedAt")]
    public DateTime CreatedAt { get; set; }
    
    [Column("ModifiedAt")]
    public DateTime UpdatedAt { get; set; }
    
    [Column("RemovedAt")]
    public DateTime? DeletedAt { get; set; }
}