using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class CreatedAtUtcEntity : BaseEntity, ICreatedAtUtc
{
    [Precision(6)] public DateTime CreatedAt { get; set; }
}