using Idam.EntityFrameworkCore.Timestamps.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Idam.EntityFrameworkCore.Timestamps.Extensions;

public static class DbContextOptionsBuilderExtensions
{
    private static readonly TimeStampsInterceptor _timeStampsInterceptor = new();

    extension(DbContextOptionsBuilder optionsBuilder)
    {
        /// <summary>
        ///     Add TimeStampsInterceptor to the DbContextOptionsBuilder.
        /// </summary>
        public void AddTimeStampsInterceptor()
        {
            optionsBuilder.AddInterceptors(_timeStampsInterceptor);
        }

        /// <summary>
        ///     Add TimeStampsInterceptor to the DbContextOptionsBuilder, reading the time from
        ///     <paramref name="timeProvider" /> instead of the system clock.
        /// </summary>
        /// <param name="timeProvider">The clock the timestamps are read from.</param>
        public void AddTimeStampsInterceptor(TimeProvider timeProvider)
        {
            optionsBuilder.AddInterceptors(new TimeStampsInterceptor(timeProvider));
        }
    }
}
