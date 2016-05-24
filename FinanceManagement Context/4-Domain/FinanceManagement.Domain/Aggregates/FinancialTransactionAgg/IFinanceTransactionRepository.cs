using System;
using System.Collections.Generic;
using System.Domain;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement.Domain.Aggregates.FinancialTransactionAgg
{
    public interface IFinanceTransactionRepository : IRepository<FinancialTransaction>
    {
        DateTime? GetProcessedMaxDate<TEntity>() where TEntity : FinancialTransaction;
        int GetNewTransactionRefNo<TEntity>() where TEntity : FinancialTransaction;
        int GetNewTransactionRefNo<TEntity>(DateTime ProcessedDate) where TEntity : FinancialTransaction;
        TEntity GetTransaction<TEntity>(Guid TransactionId) where TEntity : FinancialTransaction;
        TEntity GetTransaction<TEntity>(int TransactionNo) where TEntity : FinancialTransaction;
        IEnumerable<FinancialTransaction> GetAllTransactions(DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount);
        IEnumerable<TEntity> GetTransactions<TEntity>(DateTime ProcessedDate, int pageIndex, int pageSize, out int TotalRowCount) where TEntity : FinancialTransaction;
        IEnumerable<TEntity> GetTransactions<TEntity>(DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount) where TEntity : FinancialTransaction;
        IEnumerable<TEntity> GetTransactions<TEntity>(string FunderName, string CustomerName, int CustomerNo, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount)where TEntity:FinancialTransaction;
        IEnumerable<TEntity> GetTransactions<TEntity>(Guid FunderId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount) where TEntity : FinancialTransaction;
        IEnumerable<TEntity> GetTransactionsByCustomer<TEntity>(Guid CustomerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount) where TEntity : FinancialTransaction;
        IEnumerable<TEntity> GetTransactionsByFunder<TEntity>(Guid FunderId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount) where TEntity : FinancialTransaction;
        IEnumerable<CreditNote> GetCreditNotesForReceiptsByFunder(Guid FunderId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount);
        
    }
}
