using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class CreatedAtUtcEntity : BaseEntity, ICreatedAtUtc
{
    public DateTime CreatedAt { get; set; }
}