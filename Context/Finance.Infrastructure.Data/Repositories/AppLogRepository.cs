﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Infrastructure.Data;
using Finance.Infrastructure.Data.UnitOfWork;
using Finance.Domain;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Business Repository implementation.
    /// <see cref="HR.Domain.IAppLogRepository"/>
    /// </summary>
    public class AppLogRepository : Repository<AppLog>, IAppLogRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public AppLogRepository(IFinanceDbContext unitOfWork) : base(unitOfWork) { }

        public IEnumerable<AppLog> GetLogs()
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.AppLogs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<AppLog> GetLogs(DateTime FromLogDate, DateTime ToLogDate)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.AppLogs.Where(a=> 
                DbFunctions.TruncateTime(a.LogDateTime)>= DbFunctions.TruncateTime(FromLogDate)
                &&
                DbFunctions.TruncateTime(a.LogDateTime) <= DbFunctions.TruncateTime(ToLogDate)
                );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<AppLog> GetLogs(string User)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.AppLogs.Where(a => string.Equals(a.LogUser == User, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<AppLog> GetLogs(string User, DateTime FromLogDate, DateTime ToLogDate)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.AppLogs.Where(a => string.Equals(a.LogUser == User, StringComparison.OrdinalIgnoreCase)
                &&
                DbFunctions.TruncateTime(a.LogDateTime) >= DbFunctions.TruncateTime(FromLogDate)
                &&
                DbFunctions.TruncateTime(a.LogDateTime) <= DbFunctions.TruncateTime(ToLogDate)
                );
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}