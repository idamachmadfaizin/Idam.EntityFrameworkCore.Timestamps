namespace Idam.EFTimestamps.Tests.Models;
internal class UpdateResult<TEntity, TTimestamp>
    where TEntity : class
{
    /// <summary></summary>
    public TEntity Entity { get; set; } = default!;

    /// <summary></summary>
    public TTimestamp OldUpdatedAt { get; set; } = default!;
}
