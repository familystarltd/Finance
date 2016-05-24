using FinanceManagement.Domain;
using FinanceManagement.Web.Model;
using System;
using System.Collections.Generic;

namespace FinanceManagement.Application.Service
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