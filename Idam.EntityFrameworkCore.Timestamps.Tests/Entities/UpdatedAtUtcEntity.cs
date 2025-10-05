using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class UpdatedAtUtcEntity : BaseEntity, IUpdatedAtUtc
{
    public DateTime UpdatedAt { get; set; }
}