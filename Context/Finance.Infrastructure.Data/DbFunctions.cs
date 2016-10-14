using System;

namespace Finance.Infrastructure.Data.Repositories
{
    public class DbFunctions
    {
        public static DateTime? TruncateTime(DateTime? logDateTime)
        { 
            if (logDateTime.HasValue && logDateTime.Value != DateTime.MinValue)
                return new DateTime(logDateTime.Value.Year, logDateTime.Value.Month, logDateTime.Value.Day);
            return null;
        }
    }
}