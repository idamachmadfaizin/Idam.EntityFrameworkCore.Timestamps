using Idam.EntityFrameworkCore.Timestamps.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Entities;

/// <summary>
///     Custom column names, the scenario advertised in the README.
/// </summary>
public class RenamedColumns : BaseEntity, ITimeStamps, ISoftDelete
{
    [Column("AddedAt")] [Precision(6)] public DateTime CreatedAt { get; set; }

    [Column("ModifiedAt")] [Precision(6)] public DateTime UpdatedAt { get; set; }

    [Column("RemovedAt")] [Precision(6)] public DateTime? DeletedAt { get; set; }
}
