using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class UpdatedAtUtcEntity : BaseEntity, IUpdatedAtUtc
{
    [Precision(6)] public DateTime UpdatedAt { get; set; }
}