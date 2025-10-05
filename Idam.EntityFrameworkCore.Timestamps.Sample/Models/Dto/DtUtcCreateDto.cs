using System.ComponentModel.DataAnnotations;

namespace Idam.EntityFrameworkCore.Timestamps.Sample.Models.Dto;

public class DtUtcCreateDto
{
    [Required] [StringLength(191)] public required string Name { get; init; }

    [StringLength(191)] public string? Description { get; init; }
}