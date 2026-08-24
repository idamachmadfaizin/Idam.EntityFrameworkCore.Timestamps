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
    }
}
