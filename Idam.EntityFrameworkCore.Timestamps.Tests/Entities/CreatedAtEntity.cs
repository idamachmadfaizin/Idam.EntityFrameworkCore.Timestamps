using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class CreatedAtEntity : BaseEntity, ICreatedAt
{
    [Precision(6)] public DateTime CreatedAt { get; set; }
}