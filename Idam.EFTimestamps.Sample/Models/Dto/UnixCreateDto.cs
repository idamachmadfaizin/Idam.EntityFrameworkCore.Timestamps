using System.ComponentModel.DataAnnotations;

namespace Idam.EFTimestamps.Sample.Models.Dto;

public class UnixCreateDto
{
    [Required] [StringLength(191)] public string Name { get; init; } = default!;

    [StringLength(191)] public string? Description { get; init; }
}