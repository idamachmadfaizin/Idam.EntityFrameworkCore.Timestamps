using System.ComponentModel.DataAnnotations;

namespace Idam.EFTimestamps.Sample.Models.Dto;

public class DtUpdateDto
{
    public int Id { get; set; }

    [StringLength(191)] public required string Name { get; set; }

    [StringLength(191)] public string? Description { get; set; }
}