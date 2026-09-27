using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

public class UpdatedAtEntity : BaseEntity, IUpdatedAt
{
    [Precision(6)] public DateTime UpdatedAt { get; set; }
}