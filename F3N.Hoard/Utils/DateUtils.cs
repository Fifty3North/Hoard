using System;

namespace F3N.Hoard.Utils
{
    public class DateUtils
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static string GetRelativeTime(DateTime yourDate)
        {
            const int second = 1;
            const int minute = 60 * second;
            const int hour = 60 * minute;
            const int day = 24 * hour;
            const int month = 30 * day;

            var ts = DateTime.Now.ToLocalTime().Subtract(yourDate.ToLocalTime());
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * minute)
                return "Just now";//ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * minute)
                return "a minute ago";

            if (delta < 45 * minute)
                return ts.Minutes == 1 ? "one minute ago" : ts.Minutes + " minutes ago";

            if (delta < 90 * minute)
                return "an hour ago";

            if (delta < 24 * hour)
                return ts.Hours == 1 ? "one hour ago" : ts.Hours + " hours ago";

            if (delta < 48 * hour)
                return "yesterday";

            if (delta < 30 * day)
                return ts.Days == 1 ? "one day ago" : ts.Days + " days ago";


            if (delta < 12 * month)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        /// <summary>
        /// Converts a unix timestamp to DateTime in local time.
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            return Epoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        /// <summary>
        /// Converts a DateTime to a Unix timestamp.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc);
            }
            return Convert.ToInt64((dateTime - Epoch).TotalSeconds);
        }

        /// <summary>
        /// Calculates the difference in whole months between the two dates provided.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int DateDifferenceInMonths(DateTime startDate, DateTime endDate)
        {
            int months = (endDate.Year - startDate.Year) * 12;
            months += (endDate.Month - startDate.Month);
            if (endDate.Day < startDate.Day)
            {
                months -= 1;
                if (startDate.Month == 2 && startDate.Day == 29 && endDate.Month == 2 && endDate.Day == 28)
                {
                    months += 1;
                }
            }

            if (months < 0)
            {
                months = 0;
            }

            return months;
        }
    }
}
