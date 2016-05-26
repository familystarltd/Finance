using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FinanceManagement.Web.Model;
using FinanceManagement.Domain.Aggregates.FinancialTransactionAgg;
namespace FinanceManagement.Application.Service
{
    public interface IFinanceTransactionAppService
    {
        TEntityDTO GetTransaction<TEntity, TEntityDTO>(Guid TransactionId)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new();
        IEnumerable<TEntityDTO> GetTransactions<TEntity, TEntityDTO>(DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new();
        TEntityDTO GetTransaction<TEntity, TEntityDTO>(int TransactionNo)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new();

        IEnumerable<TEntityDTO> GetTransactions<TEntity, TEntityDTO>(Guid? FunderId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new();
        int CreateReceipt(ReceiptModel receipt);
        int CreateCreditNote(CreditNoteModel creditNote);
        CreditNoteModel CreateCreditNote(DateTime ProcessedDate, decimal Amount, string Description, Guid? FunderId, Guid? CustomerId, Guid? ReceiptId, InvoiceModel invoice);
        IEnumerable<CreditNoteModel> GetCreditNotesForReceiptsByFunder(Guid FunderId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount);

    }
}
