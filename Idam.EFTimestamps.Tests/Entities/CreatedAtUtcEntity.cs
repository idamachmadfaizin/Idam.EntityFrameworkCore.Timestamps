using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

public class CreatedAtUtcEntity : BaseEntity, ICreatedAtUtc
{
    public DateTime CreatedAt { get; set; }
}