using System;
using System.Collections.Generic;
using FinanceManagement.Domain;
using FinanceManagement.Web.Model;
using System.Infrastructure.CrossCutting.Framework.Extensions;

namespace FinanceManagement.Application.Service
{
    public class AppLogService : IAppLogService
    {
        readonly IAppLogRepository _AppLogRepository;
        public AppLogService(IAppLogRepository AppLogRepository)
        {
            _AppLogRepository = AppLogRepository;
        }
        public AppLogModel Log(AppLogModel AppLog)
        {
            try
            {
                AppLog appLog = _AppLogRepository.Get(AppLog.Id);
                if (appLog == null)
                {
                    AppLog.Id = Guid.NewGuid();
                    appLog = DataProjections.ProjectedTo<AppLog>(AppLog);
                    _AppLogRepository.Add(appLog);
                }
                else
                {
                    _AppLogRepository.Merge(appLog, DataProjections.ProjectedTo<AppLog>(AppLog));
                }
                _AppLogRepository.UnitOfWork.Commit();
                return AppLog;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<AppLog> GetLogs() {
            try{
                return _AppLogRepository.GetLogs();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<AppLog> GetLogs(DateTime FromLogDate, DateTime ToLogDate) {
            try
            {
                return _AppLogRepository.GetLogs(FromLogDate, ToLogDate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<AppLog> GetLogs(string User) {
            try
            {
                return _AppLogRepository.GetLogs(User);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<AppLog> GetLogs(string User, DateTime FromLogDate, DateTime ToLogDate) {
            try
            {
                return _AppLogRepository.GetLogs(User, FromLogDate, ToLogDate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}