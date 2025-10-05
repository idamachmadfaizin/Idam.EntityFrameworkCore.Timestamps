using Idam.EntityFrameworkCore.Timestamps.Interfaces;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class UpdatedAtUnixEntity : BaseEntity, IUpdatedAtUnix
{
    public long UpdatedAt { get; set; }
}