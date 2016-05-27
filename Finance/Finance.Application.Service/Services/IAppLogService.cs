using Finance.Domain;
using Finance.Web.Model;
using System;
using System.Collections.Generic;

namespace Finance.Application.Service
{
    public interface IAppLogService
    {
        AppLogModel Log(AppLogModel AppLog);
        IEnumerable<AppLog> GetLogs();
        IEnumerable<AppLog> GetLogs(DateTime FromLogDate, DateTime ToLogDate);
        IEnumerable<AppLog> GetLogs(string User);
        IEnumerable<AppLog> GetLogs(string User, DateTime FromLogDate, DateTime ToLogDate);
    }
}