using Idam.Libs.EF.Interfaces;
using Idam.Libs.EF.Tests.Entities;

namespace Idam.Libs.EF.Sample.Models.Entity;

/// <summary>
/// The DateTime entity.
/// </summary>
/// <seealso cref="ITimeStamps" />
/// <seealso cref="ISoftDelete" />
public class Dt : BaseEntity, ITimeStamps, ISoftDelete
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}
