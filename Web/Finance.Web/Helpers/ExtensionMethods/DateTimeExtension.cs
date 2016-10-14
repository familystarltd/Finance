using System;
namespace Finance.Web.Helpers.ExtensionMethods
{
    public static class DateExtension
    {
        private const int MONDAY = 1;
        private const int TUESDAY = 2;
        private const int WEDNESDAY = 3;
        private const int THURSDAY = 4;
        private const int FRIDAY = 5;
        private const int SATURDAY = 6;
        private const int SUNDAY = 7;

        private static string GetWeekDayFullName(this DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "Monday";
                case DayOfWeek.Tuesday:
                    return "Tuesday";
                case DayOfWeek.Wednesday:
                    return "Wednesday";
                case DayOfWeek.Thursday:
                    return "Thursday";
                case DayOfWeek.Friday:
                    return "Friday";
                case DayOfWeek.Saturday:
                    return "Saturday";
                case DayOfWeek.Sunday:
                    return "Sunday";
                default:
                    throw new ArgumentException("Invalid day!");
            }
        }

        /// <summary>
        /// Observe that the method calculates by including the start day, and end day
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static void GetWeekendDaysBetween(this DateTime date, DateTime startDate, DateTime endDate, out int saturdays, out int sundays)
        {
            saturdays = -1;
            sundays = -1;
            endDate = endDate.AddDays(1);// include end day as well
            if (endDate < startDate)
                return;
            TimeSpan timeBetween = endDate.Subtract(startDate);
            int weekendsBetween = timeBetween.Days / 7;
            sundays = weekendsBetween;
            saturdays = weekendsBetween;
            int startDay = GetDayOfWeekNumber(startDate.DayOfWeek);
            int endDay = GetDayOfWeekNumber(endDate.DayOfWeek);
            if (startDay > endDay)
            {
                sundays++;
                saturdays += (startDay < SUNDAY) ? 1 : 0;
            }
            else if (startDay < endDay)
            {
                //We don't have to care about sundays here, since we are excluding the last day
                //There will only be another saturday, if the end day is a sunday
                saturdays += (endDay == SUNDAY) ? 1 : 0;
            }
        }


        /// <summary>
        /// Since I don't think it's a good idea to rely on the face that the enums have specific values, I wrote this method
        /// If you are satisfied with using the integer value of the enums, just remember that Sundays will then have the value 0
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private static int GetDayOfWeekNumber(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday:
                    return MONDAY;
                case DayOfWeek.Tuesday:
                    return TUESDAY;
                case DayOfWeek.Wednesday:
                    return WEDNESDAY;
                case DayOfWeek.Thursday:
                    return THURSDAY;
                case DayOfWeek.Friday:
                    return FRIDAY;
                case DayOfWeek.Saturday:
                    return SATURDAY;
                case DayOfWeek.Sunday:
                    return SUNDAY;
                default:
                    throw new ArgumentException("Invalid day!");
            }
        }

        /// <summary>
        /// find the start of the week by given Day of the week
        /// </summary>
        /// <param name="startOfWeek"></param>
        /// <returns></returns>
        public static DateTime StartOfWeekDate(this DateTime dt, DayOfWeek startOfWeek,out DateTime EndOfWeekDate)
        {
            int diff = GetDayOfWeekNumber(dt.DayOfWeek) - GetDayOfWeekNumber(startOfWeek);
            if (diff < 0)
            {
                diff += 7;
            }
            DateTime startDate = dt.AddDays(-1 * diff).Date;
            EndOfWeekDate = startDate.AddDays(6);
            return startDate;
        }
        public static string ToFormatString(this TimeSpan ts)
        {
            if (ts.Days >= 1)
                return string.Format("{0}:{1}", ((ts.Days * 24) + ts.Hours), ts.Minutes.ToString("D2"));
            else
               return ts.ToString("hh':'mm");
            //string format = string.Format("{0}:{1}",ts.TotalHours,ts.Minutes); //ts.Days >= 1 ? "d'.'hh':'mm" : "hh':'mm";
            //return format; //ts.ToString(format);
        }
    }
}