using System;

namespace Yakari
{
    /// <summary>
    ///     CacheTime as TimeSpan
    /// </summary>
    public class CacheTime
    {
        /// <summary>
        ///     1 min
        /// </summary>
        public static TimeSpan OneMinute = TimeSpan.FromMinutes(1);

        /// <summary>
        ///     15 min
        /// </summary>
        public static TimeSpan FifteenMinutes = TimeSpan.FromMinutes(15);

        /// <summary>
        ///     30 min
        /// </summary>
        public static TimeSpan ThirtyMinutes = TimeSpan.FromMinutes(30);

        /// <summary>
        ///     1 hour
        /// </summary>
        public static TimeSpan OneHour = TimeSpan.FromHours(1);

        /// <summary>
        ///     3 hour
        /// </summary>
        public static TimeSpan ThreeHours = TimeSpan.FromHours(3);

        /// <summary>
        ///     6 hour
        /// </summary>
        public static TimeSpan SixHours = TimeSpan.FromHours(6);

        /// <summary>
        ///     12 hour
        /// </summary>
        public static TimeSpan TwelveHours = TimeSpan.FromHours(12);

        /// <summary>
        ///     1 day
        /// </summary>
        public static TimeSpan OneDay = TimeSpan.FromDays(1);

        /// <summary>
        ///     1 Week
        /// </summary>
        public static TimeSpan OneWeek = TimeSpan.FromDays(7);

        /// <summary>
        ///     30 days
        /// </summary>
        public static TimeSpan OneMonth = TimeSpan.FromDays(30);

        /// <summary>
        ///     365 days
        /// </summary>
        public static TimeSpan OneYear = TimeSpan.FromDays(365);
    }
}