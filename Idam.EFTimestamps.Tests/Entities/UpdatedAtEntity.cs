using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

public class UpdatedAtEntity : BaseEntity, IUpdatedAt
{
    public DateTime UpdatedAt { get; set; }
}