using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     Soft delete without any timestamp interface — a valid combination on its own.
/// </summary>
public class SoftDeleteOnly : BaseEntity, ISoftDelete
{
    [Precision(6)] public DateTime? DeletedAt { get; set; }
}
