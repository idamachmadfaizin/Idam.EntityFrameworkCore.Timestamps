using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class CreatedAtUnixEntity : BaseEntity, ICreatedAtUnix
{
    public long CreatedAt { get; set; }
}