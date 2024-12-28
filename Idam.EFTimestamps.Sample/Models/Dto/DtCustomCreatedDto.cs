using System.ComponentModel.DataAnnotations;

namespace Idam.EFTimestamps.Sample.Models.Dto;

public class DtCustomCreatedDto
{
    [Required] [StringLength(191)] public required string Name { get; init; }

    [StringLength(191)] public string? Description { get; init; }
}