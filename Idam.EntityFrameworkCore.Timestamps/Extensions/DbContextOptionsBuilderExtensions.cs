using Idam.EntityFrameworkCore.Timestamps.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

/// <summary>
///     DbContextOptionsBuilder extension class.
/// </summary>
public static class DbContextOptionsBuilderExtensions
{
    extension(DbContextOptionsBuilder optionsBuilder)
    {
        /// <summary>
        ///     Registers the timestamps save changes interceptor.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DbContextOptionsBuilder AddTimestampsInterceptor()
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            optionsBuilder.AddInterceptors(new TimestampsSaveChangesInterceptor());
            return optionsBuilder;
        }
    }

    extension<TContext>(DbContextOptionsBuilder<TContext> optionsBuilder) where TContext : DbContext
    {
        /// <summary>
        ///     Registers the timestamps save changes interceptor.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DbContextOptionsBuilder<TContext> AddTimestampsInterceptor()
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            optionsBuilder.AddInterceptors(new TimestampsSaveChangesInterceptor());
            return optionsBuilder;
        }
    }
}
