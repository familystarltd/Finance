using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Finance.Web.Model;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
namespace Finance.Application.Service
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

        IEnumerable<TEntityDTO> GetTransactions<TEntity, TEntityDTO>(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new();
        ReceiptModel GetReceipt(int ReceiptNo);
        IEnumerable<ReceiptModel> GetReceipts(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount);
        int CreateReceipt(ReceiptModel receipt);
        int CreateCreditNote(CreditNoteModel creditNote);
        CreditNoteModel CreateCreditNote(DateTime ProcessedDate, decimal Amount, string Description, Guid? PayerId, Guid? CustomerId, Guid? ReceiptId, InvoiceModel invoice);
        IEnumerable<CreditNoteModel> GetCreditNotes(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount);
        IEnumerable<CreditNoteModel> GetCreditNotesForReceiptsByPayer(Guid PayerId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount);
        CreditNoteModel GetCreditNote(int creditNoteNo);
    }
}
