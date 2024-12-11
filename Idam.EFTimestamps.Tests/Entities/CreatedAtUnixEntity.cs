using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

public class CreatedAtUnixEntity : BaseEntity, ICreatedAtUnix
{
    public long CreatedAt { get; set; }
}