using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

public class CreatedAtEntity : BaseEntity, ICreatedAt
{
    public DateTime CreatedAt { get; set; }
}