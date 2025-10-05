using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Idam.EntityFrameworkCore.Timestamps.Sample.Models.Dto;

namespace Idam.EntityFrameworkCore.Timestamps.Sample.Models.Entity;

/// <summary>
///     The Unix entity
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
        Name = dto.Name;
        Description = dto.Description;
    }

    [SetsRequiredMembers]
    public Unix(UnixUpdateDto dto)
    {
        Id = dto.Id;
        Name = dto.Name;
        Description = dto.Description;
    }

    [Key] public int Id { get; set; }

    [StringLength(191)] public required string Name { get; set; }

    [StringLength(191)] public string? Description { get; set; }

    public long? DeletedAt { get; set; }

    public long CreatedAt { get; set; }

    public long UpdatedAt { get; set; }
}