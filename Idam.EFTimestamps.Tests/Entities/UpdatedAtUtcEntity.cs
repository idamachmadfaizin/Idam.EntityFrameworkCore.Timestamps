using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

public class UpdatedAtUtcEntity : BaseEntity, IUpdatedAtUtc
{
    public DateTime UpdatedAt { get; set; }
}