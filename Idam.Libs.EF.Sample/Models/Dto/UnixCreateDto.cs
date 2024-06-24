using System.ComponentModel.DataAnnotations;

namespace Idam.Libs.EF.Sample.Models.Dto;

public class UnixCreateDto
{
    [Required]
    [StringLength(191)]
    public string Name { get; set; } = default!;

    [StringLength(191)]
    public string? Description { get; set; }
}
