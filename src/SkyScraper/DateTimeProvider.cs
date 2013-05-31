using System;

namespace SkyScraper
{
    public static class DateTimeProvider
    {
        static Func<DateTime> nowFunc = () => DateTime.UtcNow;

        public static DateTime UtcNow
        {
            get { return nowFunc(); }
            set { nowFunc = () => value; }
        }
    }
}