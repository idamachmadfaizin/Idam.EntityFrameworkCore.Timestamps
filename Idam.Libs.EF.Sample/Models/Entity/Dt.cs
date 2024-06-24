using Idam.Libs.EF.Interfaces;
using Idam.Libs.EF.Sample.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Idam.Libs.EF.Sample.Models.Entity;

/// <summary>
/// The DateTime entity.
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

    [Key]
    public int Id { get; set; }

    [StringLength(191)]
    public required string Name { get; set; }

    [StringLength(191)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
