using Idam.EntityFrameworkCore.Timestamps.Tests.Context;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Tests.Fixtures;

/// <summary>
///     Supplies a database for one test. Implementations that need a container start it once per
///     collection; every test still gets its own database so tests stay isolated.
/// </summary>
public interface IDbFixture
{
    /// <summary>
    ///     Builds the options for a fresh, empty database with the given name.
    /// </summary>
    DbContextOptions<TestDbContext> BuildOptions(string databaseName);
}
