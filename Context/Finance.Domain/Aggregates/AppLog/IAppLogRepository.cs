using System;
using System.Collections.Generic;
using System.Domain;

namespace Finance.Domain
{
    public interface IAppLogRepository : IRepository<AppLog>
    {
        IEnumerable<AppLog> GetLogs();
        IEnumerable<AppLog> GetLogs(DateTime FromLogDate,DateTime ToLogDate);
        IEnumerable<AppLog> GetLogs(string User);
        IEnumerable<AppLog> GetLogs(string User, DateTime FromLogDate, DateTime ToLogDate);
    }
}
