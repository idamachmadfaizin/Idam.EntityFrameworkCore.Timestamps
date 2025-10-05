using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class CreatedAtEntity : BaseEntity, ICreatedAt
{
    public DateTime CreatedAt { get; set; }
}