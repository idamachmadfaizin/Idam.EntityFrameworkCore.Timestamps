using Idam.Libs.EF.Interfaces;
using Idam.Libs.EF.Sample.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Idam.Libs.EF.Sample.Models.Entity;

/// <summary>
/// The Unix entity
/// </summary>
/// <seealso cref="ITimeStampsUnix" />
/// <seealso cref="ISoftDeleteUnix" />
public class Unix : ITimeStampsUnix, ISoftDeleteUnix
{
    public Unix()
    {
    }

    [SetsRequiredMembers]
    public Unix(UnixCreateDto dto)
    {
        this.Name = dto.Name;
        this.Description = dto.Description;
    }

    [SetsRequiredMembers]
    public Unix(UnixUpdateDto dto)
    {
        this.Id = dto.Id;
        this.Name = dto.Name;
        this.Description = dto.Description;
    }

    [Key]
    public int Id { get; set; }

    [StringLength(191)]
    public required string Name { get; set; }

    [StringLength(191)]
    public string? Description { get; set; }

    public long CreatedAt { get; set; }

    public long UpdatedAt { get; set; }

    public long? DeletedAt { get; set; }
}