using System;
using System.Linq;
using System.Collections.Generic;
using System.Infrastructure.Data;
using Finance.Infrastructure.Data.UnitOfWork;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using Microsoft.EntityFrameworkCore;

namespace Finance.Infrastructure.Data.Repositories
{
    /// <summary>
    /// The Customer Repository implementation.
    /// <see cref="Finance.Domain.IFinanceTransactionRepository"/>
    /// </summary>
    public class FinanceTransactionRepository : Repository<FinancialTransaction>, IFinanceTransactionRepository
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        /// <param name="unitOfWork">Associated unit of work</param>
        public FinanceTransactionRepository(IFinanceDbContext unitOfWork) : base(unitOfWork) { }
        public override void Merge(FinancialTransaction persisted, FinancialTransaction current)
        {
            base.Merge(persisted, current);
        }

        public DateTime? GetProcessedMaxDate<TEntity>() where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
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
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<TEntity>().Where(trans => trans.Id == TransactionId).SingleOrDefault();
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
                var uow = this.UnitOfWork as IFinanceDbContext;
                TEntity entity = uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Customer).ThenInclude(c => c.Fees)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<TEntity>().Where(trans => trans.TransactionRef == TransactionNo).SingleOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public int GetNewTransactionRefNo<TEntity>() where TEntity : FinancialTransaction
        {
            var uow = this.UnitOfWork as IFinanceDbContext;
            int? maxTransactionRefNo = uow.FinancialTransactions.OfType<TEntity>().Max(tr => (int?)tr.TransactionRef);
            return !maxTransactionRefNo.HasValue || maxTransactionRefNo <= 1000 ? 1001 : maxTransactionRefNo.Value + 1;
        }
        public int GetNewTransactionRefNo<TEntity>(DateTime ProcessedDate) where TEntity : FinancialTransaction
        {
            var uow = this.UnitOfWork as IFinanceDbContext;
            if (uow.FinancialTransactions.OfType<TEntity>().Count(tr => DbFunctions.TruncateTime(tr.ProcessedDate) <= ProcessedDate.Date && DbFunctions.TruncateTime(tr.ProcessedDate) >= ProcessedDate.Date) > 0)
                return -1;
            int? maxTransactionRefNo = uow.FinancialTransactions.OfType<TEntity>().Max(tr => (int?)tr.TransactionRef);
            return !maxTransactionRefNo.HasValue || maxTransactionRefNo <= 1000 ? 1001 : maxTransactionRefNo.Value + 1;
        }
        public IEnumerable<FinancialTransaction> GetAllTransactions(DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.FinancialTransactions.Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || (!FromDate.HasValue || !ToDate.HasValue)
                    );
                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
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
            var uow = this.UnitOfWork as IFinanceDbContext;
            TotalRowCount = uow.FinancialTransactions.OfType<TEntity>().Count(tr => DbFunctions.TruncateTime(tr.ProcessedDate) == DbFunctions.TruncateTime(ProcessedDate));
            return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<TEntity>()
                .Where(tr => DbFunctions.TruncateTime(tr.ProcessedDate) == DbFunctions.TruncateTime(ProcessedDate))
                .OrderBy(tr => tr.TransactionRef)
                .Skip(pageIndex * pageSize)
                .Take(pageSize);
        }
        public IEnumerable<TEntity> GetTransactions<TEntity>(DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.FinancialTransactions.OfType<TEntity>().Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || (!FromDate.HasValue || !ToDate.HasValue)
                    );
                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<TEntity>()
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
        public IEnumerable<TEntity> GetTransactions<TEntity>(string PayerName, string CustomerName, int CustomerNo, DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            var uow = this.UnitOfWork as IFinanceDbContext;
            PayerName = string.IsNullOrEmpty(PayerName) ? "" : PayerName;
            CustomerName = string.IsNullOrEmpty(CustomerName) ? "" : CustomerName;
            //if(!string.IsNullOrEmpty(CustomerName))
            //    CustomerName = CustomerName.Trim().Replace(" ", ", ");
            TotalRowCount = uow.FinancialTransactions.OfType<TEntity>()
                .Count(tr => ((!string.IsNullOrEmpty(PayerName) && tr.Payer.Name.Equals(PayerName.Trim(), StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(PayerName))
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
            return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<TEntity>()
                    .Where(tr => ((!string.IsNullOrEmpty(PayerName) && tr.Payer.Name.Equals(PayerName.Trim(), StringComparison.OrdinalIgnoreCase)) || string.IsNullOrEmpty(PayerName))
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
                    .OrderByDescending(tr => tr.TransactionRef)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
        }
        public IEnumerable<TEntity> GetTransactions<TEntity>(Guid PayerId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.FinancialTransactions.OfType<FeeInvoice>().Count(tr => tr.Payer.Id == PayerId
                            && (DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate)));

                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<TEntity>()
                    .Where(tr => tr.Payer.Id == PayerId
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
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.FinancialTransactions.OfType<TEntity>().Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) && tr.CustomerId == CustomerId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.CustomerId == CustomerId
                    );
                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<TEntity>()
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
        public IEnumerable<TEntity> GetTransactionsByPayer<TEntity>(Guid PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount) where TEntity : FinancialTransaction
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.FinancialTransactions.OfType<TEntity>().Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) && tr.PayerId == PayerId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.PayerId == PayerId
                    );
                return uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<TEntity>()
                    .Where(tr =>
                        ((FromDate.HasValue && ToDate.HasValue) && tr.PayerId == PayerId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.PayerId == PayerId
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
        public Receipt GetReceipt(int ReceiptNo)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                Receipt entity = uow.Receipts
                    .Include(r=>r.CreditNote)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .Include(r=>r.ReceiptInvoices).ThenInclude(ri=>ri.Invoice).ThenInclude(inv=>inv.ReceiptInvoices).ThenInclude(r=>r.Receipt)
                    .Where(trans => trans.TransactionRef == ReceiptNo).SingleOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<Receipt> GetReceipts(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.Receipts.Count(tr =>
                ((PayerId.HasValue && tr.PayerId == PayerId) || !PayerId.HasValue) &&
                    (
                    (FromDate.HasValue && ToDate.HasValue) 
                    &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)
                    )
                    || tr.PayerId == PayerId
                    );
                IEnumerable<Receipt> receipts = uow.Receipts
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .Include(r => r.ReceiptInvoices).ThenInclude(ri => ri.Invoice)
                    .Where(tr => ((PayerId.HasValue && tr.PayerId == PayerId) || !PayerId.HasValue) &&
                    (
                    (FromDate.HasValue && ToDate.HasValue)
                    &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value)
                    )
                    || tr.PayerId == PayerId
                            )
                            .OrderByDescending(fi => fi.ProcessedDate)
                            .Skip(PageIndex * PageSize)
                            .Take(PageSize);
                return receipts;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public CreditNote GetCreditNote(int creditNoteNo)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                return uow.CreditNotes
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .Include(cr => cr.Receipts).ThenInclude(r => r.ReceiptInvoices).ThenInclude(r => r.Receipt)
                    .Include(cr => cr.Receipts).ThenInclude(r => r.ReceiptInvoices).ThenInclude(r => r.Invoice.ReceiptInvoices)
                    .Include(cr => cr.Receipts).ThenInclude(r => r.ReceiptInvoices).ThenInclude(r => r.Invoice.Customer.PersonalInfo)
                    .SingleOrDefault(tr => tr.CreditNoteNo == creditNoteNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CreditNote> GetCreditNotesForReceiptsByPayer(Guid PayerId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                IEnumerable<CreditNote> creditNotes = uow.FinancialTransactions
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .OfType<CreditNote>().Where(tr =>
                   tr.PayerId == PayerId);
                TotalRowCount = creditNotes.Count(tr => (tr.Amount - (tr == null ? 0 : (tr as CreditNote).Receipts.Sum(r => (decimal?)r.Amount) ?? 0) > 0) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate));

               //TotalRowCount = uow.FinancialTransactions.OfType<CreditNote>().Where(tr =>
               //     tr.PayerId == PayerId &&
               //     (tr.Amount - (tr == null ? 0 : (tr as CreditNote).Receipts.Sum(r => (decimal?)r.Amount) ?? 0) > 0) &&
               //     DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate)).Count();
               return creditNotes.Where(tr =>
                    tr.PayerId == PayerId &&
                    //(inv.Amount - (inv.ReceiptInvoices.Sum(r => (decimal?)r.AmountReceived) ?? 0) > 0));
                    (tr.Amount - ((tr as CreditNote).Receipts.Sum(r => (decimal?)r.Amount) ?? 0) > 0) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate))
                    .OrderByDescending(tr => tr.ProcessedDate)
                    .Skip(PageIndex * PageSize)
                    .Take(PageSize);
                //return uow.FinancialTransactions.OfType<CreditNote>()
                    
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CreditNote> GetCreditNotes(DateTime? FromDate, DateTime? ToDate, int pageIndex, int pageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.CreditNotes.Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || (!FromDate.HasValue || !ToDate.HasValue)
                    );
                return uow.CreditNotes
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .Include(cr => cr.Receipt)
                    .Include(cr => cr.Invoice).ThenInclude(inv => inv.InvoiceDetails)
                    .Include(cr => cr.Invoice).ThenInclude(inv => inv.ReceiptInvoices).ThenInclude(r => r.Receipt)
                    .Include(cr => cr.Receipts)
                    .Where(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || (!FromDate.HasValue || !ToDate.HasValue)
                    )
                    .OrderByDescending(tr => tr.ProcessedDate)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CreditNote> GetCreditNotes(Guid PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                var uow = this.UnitOfWork as IFinanceDbContext;
                TotalRowCount = uow.CreditNotes.Count(tr =>
                    ((FromDate.HasValue && ToDate.HasValue) && tr.PayerId == PayerId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.PayerId == PayerId
                    );
                return uow.CreditNotes
                    .Include(tra => tra.Customer).ThenInclude(c => c.PersonalInfo)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.DisbursementInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceBillingContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.FeeInvoiceDeliveryContact)
                    .Include(tra => tra.Payer).ThenInclude(p => p.PersonalContact)
                    .Include(cr => cr.Receipt)
                    .Include(cr => cr.Invoice).ThenInclude(inv=>inv.InvoiceDetails)
                    .Include(cr => cr.Invoice).ThenInclude(inv => inv.ReceiptInvoices).ThenInclude(r=>r.Receipt)
                    .Include(cr=>cr.Receipts)
                    .Where(tr =>
                        ((FromDate.HasValue && ToDate.HasValue) && tr.PayerId == PayerId &&
                    DbFunctions.TruncateTime(tr.ProcessedDate) >= DbFunctions.TruncateTime(FromDate.Value) && DbFunctions.TruncateTime(tr.ProcessedDate) <= DbFunctions.TruncateTime(ToDate.Value))
                    || tr.PayerId == PayerId
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
    }
}