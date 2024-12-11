using Idam.EFTimestamps.Interfaces;

namespace Idam.EFTimestamps.Tests.Entities;

public class UpdatedAtUnixEntity : BaseEntity, IUpdatedAtUnix
{
    public long UpdatedAt { get; set; }
}