using System;
using System.Linq;
using System.Collections.Generic;
using System.Infrastructure.Data;
using FinanceManagement.Infrastructure.Data.UnitOfWork;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;

namespace FinanceManagement.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Customer Repository implementation.
    /// <see cref="FinanceManagement.Domain.IFinanceTransactionRepository"/>
    /// </summary>
    public class FinanceTransactionRepository : Repository<FinancialTransaction>, IFinanceTransactionRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public FinanceTransactionRepository(FinanceManagementContext unitOfWork) : base(unitOfWork) { }
        public override void Merge(FinancialTransaction persisted, FinancialTransaction current)
        {
            base.Merge(persisted, current);
        }

        public DateTime? GetProcessedMaxDate<TEntity>() where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                DateTime? maxProcessedDate = uow.FinancialTransactions.OfType<TEntity>().Max(inv => (DateTime?)inv.ProcessedDate);
                return maxProcessedDate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public TEntity GetTransaction<TEntity>(Guid TransactionId) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                return uow.FinancialTransactions.OfType<TEntity>().Where(trans => trans.Id == TransactionId).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public TEntity GetTransaction<TEntity>(int TransactionNo) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TEntity entity = uow.FinancialTransactions.OfType<TEntity>().Where(trans => trans.TransactionRef == TransactionNo).SingleOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public int GetNewTransactionRefNo<TEntity>() where TEntity : FinancialTransaction
        {
            var uow = this.UnitOfWork as FinanceManagementContext;
            int? maxTransactionRefNo = uow.FinancialTransactions.OfType<TEntity>().Max(tr => (int?)tr.TransactionRef);
            return !maxTransactionRefNo.HasValue || maxTransactionRefNo <= 1000 ? 1001 : maxTransactionRefNo.Value + 1;
        }
        public int GetNewTransactionRefNo<TEntity>(DateTime ProcessedDate) where TEntity : FinancialTransaction
        {
            var uow = this.UnitOfWork as FinanceManagementContext;
            if (uow.FinancialTransactions.OfType<TEntity>().Count(tr => DbFunctions.TruncateTime(tr.ProcessedDate) <= ProcessedDate.Date && DbFunctions.TruncateTime(tr.ProcessedDate) >= ProcessedDate.Date) > 0)
                return -1;
            int? maxTransactionRefNo = uow.FinancialTransactions.OfType<TEntity>().Max(tr => (int?)tr.TransactionRef);
            return !maxTransactionRefNo.HasValue || maxTransactionRefNo <= 1000 ? 1001 : maxTransactionRefNo.Value + 1;
        }
        public IEnumerable<FinancialTransaction> GetAllTransactions(DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.FinancialTransactions.Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || (!FromDate.HasValue || !ToDate.HasValue)
                    );
                return uow.FinancialTransactions
                    .Where(tr =>
                        ((FromDate.HasValue && ToDate.HasValue) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || (!FromDate.HasValue || !ToDate.HasValue))
                    .OrderByDescending(tr => tr.ProcessedDate)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<TEntity> GetTransactions<TEntity>(DateTime ProcessedDate, int pageIndex, int pageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            var uow = this.UnitOfWork as FinanceManagementContext;
            TotalRowCount = uow.FinancialTransactions.OfType<TEntity>().Count(tr => DbFunctions.TruncateTime(tr.ProcessedDate) == DbFunctions.TruncateTime(ProcessedDate));
            return uow.FinancialTransactions.OfType<TEntity>()
                .Where(tr => DbFunctions.TruncateTime(tr.ProcessedDate) == DbFunctions.TruncateTime(ProcessedDate))
                .OrderBy(tr => tr.TransactionRef)
                .Skip(pageIndex * pageSize)
                .Take(pageSize);
        }
        public IEnumerable<TEntity> GetTransactions<TEntity>(DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.FinancialTransactions.OfType<TEntity>().Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || (!FromDate.HasValue || !ToDate.HasValue)
                    );
                return uow.FinancialTransactions.OfType<TEntity>()
                    .Where(tr =>
                        ((FromDate.HasValue && ToDate.HasValue) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || (!FromDate.HasValue || !ToDate.HasValue)
                    )
                    .OrderBy(tr => tr.TransactionRef)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<TEntity> GetTransactions<TEntity>(string FunderName, string CustomerName, int CustomerNo, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            var uow = this.UnitOfWork as FinanceManagementContext;
            //if(!string.IsNullOrEmpty(CustomerName))
            //    CustomerName = CustomerName.Trim().Replace(" ", ", ");
            TotalRowCount = uow.FinancialTransactions.OfType<TEntity>()
                .Count(tr => ((!string.IsNullOrEmpty(FunderName) && tr.Funder.Name.Equals(FunderName.Trim(), StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(FunderName))
                            &&
                            ((!string.IsNullOrEmpty(CustomerName) && tr.Customer.PersonalInfo.FullName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(CustomerName))
                            &&
                            ((CustomerNo > 0 && tr.Customer.Ref == CustomerNo.ToString()) || CustomerNo == 0)
                            &&
                            (
                                (FromDate.HasValue && ToDate.HasValue
                                &&
                                DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value)
                                &&
                                DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                                ||
                                (!FromDate.HasValue || !ToDate.HasValue)
                            )
                    );
            return uow.FinancialTransactions.OfType<TEntity>()
                    .Where(tr => ((!string.IsNullOrEmpty(FunderName) && tr.Funder.Name.Equals(FunderName.Trim(), StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(FunderName))
                                &&
                                ((!string.IsNullOrEmpty(CustomerName) && tr.Customer.PersonalInfo.FullName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(CustomerName))
                                &&
                                ((CustomerNo > 0 && tr.Customer.Ref == CustomerNo.ToString()) || CustomerNo == 0)
                                &&
                                (
                                    (FromDate.HasValue && ToDate.HasValue
                                    &&
                                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value)
                                    &&
                                    DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                                    ||
                                    (!FromDate.HasValue && !ToDate.HasValue)
                                )
                            )
                    .OrderByDescending(tr => tr.TransactionRef).ThenBy(tr => tr.TransactionRef)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
        }
        public IEnumerable<TEntity> GetTransactions<TEntity>(Guid FunderId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.FinancialTransactions.OfType<FeeInvoice>().Count(tr => tr.Funder.Id == FunderId
                            && (DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate)));

                return uow.FinancialTransactions.OfType<TEntity>()
                    .Where(tr => tr.Funder.Id == FunderId
                            && (DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate)))
                            .OrderByDescending(fi => fi.ProcessedDate)
                            .Skip(PageIndex * PageSize)
                            .Take(PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<TEntity> GetTransactionsByCustomer<TEntity>(Guid CustomerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.FinancialTransactions.OfType<TEntity>().Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) && tr.CustomerId == CustomerId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.CustomerId == CustomerId
                    );
                return uow.FinancialTransactions.OfType<TEntity>()
                    .Where(tr =>
                        ((FromDate.HasValue && ToDate.HasValue) && tr.CustomerId == CustomerId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.CustomerId == CustomerId
                    )
                    .OrderByDescending(tr => tr.ProcessedDate)
                    .Skip(PageIndex * PageSize)
                    .Take(PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<TEntity> GetTransactionsByFunder<TEntity>(Guid FunderId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.FinancialTransactions.OfType<TEntity>().Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) && tr.FunderId == FunderId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.FunderId == FunderId
                    );
                return uow.FinancialTransactions.OfType<TEntity>()
                    .Where(tr =>
                        ((FromDate.HasValue && ToDate.HasValue) && tr.FunderId == FunderId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.FunderId == FunderId
                    )
                    .OrderByDescending(tr => tr.ProcessedDate)
                    .Skip(PageIndex * PageSize)
                    .Take(PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CreditNote> GetCreditNotesForReceiptsByFunder(Guid FunderId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as FinanceManagementContext;
                TotalRowCount = uow.FinancialTransactions.OfType<CreditNote>().Count(tr =>
                    tr.FunderId == FunderId &&
                    (tr.Amount - ((tr as CreditNote).Receipts.Sum(r => (decimal?)r.Amount) ?? 0) > 0) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate));
                return uow.FinancialTransactions.OfType<CreditNote>()
                    .Where(tr =>
                    tr.FunderId == FunderId &&
                    //(inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) > 0));
                    (tr.Amount - ((tr as CreditNote).Receipts.Sum(r => (decimal?)r.Amount) ?? 0) > 0) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate))
                    .OrderByDescending(tr => tr.ProcessedDate)
                    .Skip(PageIndex * PageSize)
                    .Take(PageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}