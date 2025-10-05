using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class UpdatedAtEntity : BaseEntity, IUpdatedAt
{
    public DateTime UpdatedAt { get; set; }
}