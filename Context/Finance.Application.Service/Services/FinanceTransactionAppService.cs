using System;
using System.Collections.Generic;
using Finance.Domain.Aggregates.FinancialTransactionAgg;
using Finance.Web.Model;
using System.Infrastructure.CrossCutting.Framework.Extensions;

namespace Finance.Application.Service
{
    public class FinanceTransactionAppService : IFinanceTransactionAppService
    {
        readonly IFinanceTransactionRepository _FinanceTransactionRepository;
        public FinanceTransactionAppService(IFinanceTransactionRepository FinanceTransactionRepository)
        {
            this._FinanceTransactionRepository = FinanceTransactionRepository;
        }

        public TEntityDTO GetTransaction<TEntity, TEntityDTO>(Guid TransactionId)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new()
        {
            try
            {
                TEntity Entity = this._FinanceTransactionRepository.GetTransaction<TEntity>(TransactionId);
                return Entity.ProjectedTo<TEntityDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public TEntityDTO GetTransaction<TEntity, TEntityDTO>(int TransactionNo)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new()
        {
            try
            {
                TEntity Entity = this._FinanceTransactionRepository.GetTransaction<TEntity>(TransactionNo);
                return Entity.ProjectedTo<TEntityDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        public IEnumerable<TEntityDTO> GetTransactions<TEntity, TEntityDTO>(DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new()
        {
            try
            {
                TotalRowCount = 0;
                IEnumerable<TEntity> transactions = this._FinanceTransactionRepository.GetTransactions<TEntity>(FromDate, ToDate, PageIndex, PageSize, out TotalRowCount);
                return transactions.ProjectedToCollection<TEntityDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<TEntityDTO> GetTransactions<TEntity, TEntityDTO>(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
            where TEntity : FinancialTransaction
            where TEntityDTO : FinancialTransactionModel, new()
        {
            try
            {
                TotalRowCount = 0;
                IEnumerable<TEntity> transactions = null;
                if (PayerId.HasValue && PayerId.Value != Guid.Empty)
                {
                    transactions = this._FinanceTransactionRepository.GetTransactionsByPayer<TEntity>(PayerId.Value, FromDate, ToDate, PageIndex, PageSize, out TotalRowCount);
                }
                else
                {
                    transactions = this._FinanceTransactionRepository.GetTransactions<TEntity>(FromDate, ToDate, PageIndex, PageSize, out TotalRowCount);
                }
                return transactions.ProjectedToCollection<TEntityDTO>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #region RECEIPTS
        public ReceiptModel GetReceipt(int ReceiptNo)
        {
            try
            {
                Receipt receipt = this._FinanceTransactionRepository.GetReceipt(ReceiptNo);
                ReceiptModel receiptModel = receipt.ProjectedTo<ReceiptModel>();
                return receiptModel;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<ReceiptModel> GetReceipts(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                IEnumerable<Receipt> receiptEntities = this._FinanceTransactionRepository.GetReceipts(PayerId, FromDate, ToDate, PageIndex, PageSize, out TotalRowCount);
                IEnumerable<ReceiptModel> receipts = receiptEntities.ProjectedToCollection<ReceiptModel>();
                return receipts;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public int CreateReceipt(ReceiptModel receiptModel)
        {
            int receiptNo = 0;
            try
            {
                Receipt receipt = _FinanceTransactionRepository.GetTransaction<Receipt>(receiptModel.Id);
                if (receipt == null)
                {
                    if (receiptModel.Amount <= 0)
                        throw new Exception("Receipt amount should be more than 0.00");
                    if (receiptModel.Id == null || receiptModel.Id == Guid.Empty)
                        receiptModel.Id = Guid.NewGuid();
                    receipt = DataProjections.ProjectedTo<Receipt>(receiptModel);

                    receipt.TransactionStatus = TransactionStatus.New;
                    decimal totalReceived = 0;
                    foreach (ReceiptInvoice receiptInvoice in receipt.ReceiptInvoices)
                    {
                        receiptInvoice.Id = Guid.NewGuid();
                        totalReceived += receiptInvoice.AmountReceived;
                        receiptInvoice.ReceiptId = receipt.Id;
                        receiptInvoice.Invoice = _FinanceTransactionRepository.GetTransaction<Invoice>(receiptInvoice.InvoiceId);
                        receiptInvoice.Invoice.Customer.Credits += receiptInvoice.AmountReceived;
                        //this._FinanceTransactionRepository.Add(receiptInvoice);
                    }
                    this._FinanceTransactionRepository.UnitOfWork.BeginTransaction();
                    receiptNo = _FinanceTransactionRepository.GetNewTransactionRefNo<Receipt>();
                    receipt.ReceiptNo = receiptNo;
                    this._FinanceTransactionRepository.Add(receipt);
                    if (receipt.PaymentMethod == ReceiptPayMethod.CreditNote)
                    {
                        if (receipt.Amount != totalReceived)
                        {
                            throw new Exception(string.Format("The Total amount you have allocated to invoices by credit note  {0} is not the same as the Amount of the receipt {1}", totalReceived, receipt.Amount));
                        }
                        if (!receipt.CreditNoteId.HasValue || receipt.CreditNoteId.Value == Guid.Empty)
                        {
                            throw new Exception(string.Format("Please select Credit Note to create receipt"));
                        }
                    }
                    else { receipt.CreditNoteId = null; }
                    if (receipt.Amount > totalReceived)
                    {
                        // Excess of receipt amount will be Created a Credit Note 
                        this.CreateCreditNote(receipt.ProcessedDate.Date, receipt.Amount - totalReceived, "Credit Note via Receipt", receipt.PayerId, receipt.CustomerId, receipt.Id, null);
                    }
                    else if (receipt.Amount < totalReceived)
                    {
                        throw new Exception(string.Format(" The Total amount you have allocated to invoices {0} is not the same as the Amount of the receipt {1}", totalReceived, receipt.Amount));
                    }
                    //receipt.CreditNote.ReceiptId = receipt.Id;
                    this._FinanceTransactionRepository.UnitOfWork.SaveChanges();
                    this._FinanceTransactionRepository.UnitOfWork.Commit();
                }
                return receiptNo;
            }
            catch (Exception ex)
            {
                this._FinanceTransactionRepository.UnitOfWork.RollBackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        Receipt CreateReceiptByCreditNote(CreditNote creditNote,decimal InvoiceCreditAmount,Invoice invoice)
        {
            Receipt receipt = new Receipt();
            receipt.PaymentMethod = ReceiptPayMethod.CreditNote;
            receipt.ContactReference = string.Format("Paid by Credit Note {0}", creditNote.CreditNoteNo);
            receipt.Amount = InvoiceCreditAmount;
            receipt.Id = Guid.NewGuid();
            receipt.ReceiptNo = _FinanceTransactionRepository.GetNewTransactionRefNo<Receipt>();
            receipt.ProcessedDate = creditNote.ProcessedDate.Date;
            receipt.CreditNote = creditNote;
            receipt.CreditNoteId = creditNote.Id;
            receipt.TransactionStatus = TransactionStatus.New;
            receipt.PayerId = creditNote.PayerId;
            receipt.CustomerId = creditNote.CustomerId;
            receipt.Description = string.Format("Receipt for invoice {0}. Paid by Credit Note {1}", invoice.InvoiceNo, creditNote.CreditNoteNo);

            ReceiptInvoice receiptInvoice = new ReceiptInvoice();
            receiptInvoice.Id = Guid.NewGuid();
            receiptInvoice.Receipt = receipt;
            receiptInvoice.ReceiptId = receipt.Id;
            receiptInvoice.Invoice = invoice;
            receiptInvoice.InvoiceId = creditNote.InvoiceId.Value;
            receiptInvoice.AmountReceived = InvoiceCreditAmount;
            invoice.Customer.Credits += receiptInvoice.AmountReceived;
            receipt.ReceiptInvoices = new List<ReceiptInvoice>();
            //invoice.ReceiptInvoices = receipt.ReceiptInvoices;
            receipt.ReceiptInvoices.Add(receiptInvoice);
            //this._FinanceTransactionRepository.Add<ReceiptInvoice>(receiptInvoice);
            //this._FinanceTransactionRepository.UnitOfWork.SaveChanges();
            return receipt;
        }
        #endregion

        #region CREDIT NOTES
        public int CreateCreditNote(CreditNoteModel creditNoteModel)
        {
            try
            {
                CreditNote creditNote = _FinanceTransactionRepository.GetTransaction<CreditNote>(creditNoteModel.Id);
                if (creditNote == null)
                {
                    if (creditNoteModel.Amount <= 0)
                        throw new Exception("CreditNote amount should be more than 0.00");
                    //creditNote = new CreditNote();
                    creditNote = DataProjections.ProjectedTo<CreditNote>(creditNoteModel);
                    creditNote.Id = Guid.NewGuid();
                    creditNote.TransactionStatus = TransactionStatus.New;
                    this._FinanceTransactionRepository.UnitOfWork.BeginTransaction();
                    creditNoteModel.CreditNoteNo = _FinanceTransactionRepository.GetNewTransactionRefNo<CreditNote>();
                    creditNote.CreditNoteNo = creditNoteModel.CreditNoteNo;
                    if (creditNote.InvoiceId.HasValue && creditNote.InvoiceId.Value != Guid.Empty)
                    {
                        Invoice invoice = _FinanceTransactionRepository.GetTransaction<Invoice>(creditNote.InvoiceId.Value);
                        if (invoice != null)
                        {
                            creditNote.Invoice = invoice;
                            //creditNoteModel.Invoice = invoice.ProjectedTo<InvoiceModel>();
                            //this._FinanceTransactionRepository.Add(this.CreateReceiptByCreditNote(creditNote, creditNoteModel.InvoiceCreditAmount, invoice));
                            creditNote.Receipts.Add(this.CreateReceiptByCreditNote(creditNote, creditNoteModel.InvoiceCreditAmount, invoice));
                        }
                    }
                    this._FinanceTransactionRepository.Add(creditNote);
                    this._FinanceTransactionRepository.UnitOfWork.SaveChanges();
                    this._FinanceTransactionRepository.UnitOfWork.Commit();
                }
                return creditNoteModel.CreditNoteNo;
            }
            catch (Exception ex)
            {
                this._FinanceTransactionRepository.UnitOfWork.RollBackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public CreditNoteModel GetCreditNote(int CreditNoteNo)
        {
            try
            {
                CreditNote creditNote = this._FinanceTransactionRepository.GetCreditNote(CreditNoteNo);
                return creditNote.ProjectedTo<CreditNoteModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public CreditNoteModel CreateCreditNote(DateTime ProcessedDate, decimal Amount, string Description, Guid? PayerId, Guid? CustomerId, Guid? ReceiptId, InvoiceModel invoice)
        {
            try
            {
                CreditNote creditNote = new CreditNote();
                creditNote.Id = Guid.NewGuid();
                creditNote.CreditNoteNo = this._FinanceTransactionRepository.GetNewTransactionRefNo<CreditNote>();
                creditNote.TransactionStatus = TransactionStatus.New;
                creditNote.Description = Description;
                creditNote.ProcessedDate = ProcessedDate;
                creditNote.Amount = Amount;
                creditNote.PayerId = PayerId;
                creditNote.ReceiptId = ReceiptId;
                creditNote.CustomerId = CustomerId;
                if (invoice != null)
                {
                    creditNote.Invoice = null;
                    creditNote.InvoiceId = invoice.Id;
                }
                this._FinanceTransactionRepository.Add(creditNote);
                this._FinanceTransactionRepository.UnitOfWork.SaveChanges();
                CreditNote credit = this._FinanceTransactionRepository.GetTransaction<CreditNote>(creditNote.CreditNoteNo);
                return credit.ProjectedTo<CreditNoteModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CreditNoteModel> GetCreditNotes(Guid? PayerId, DateTime? FromDate, DateTime? ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                IEnumerable<CreditNote> transactions = null;
                if (PayerId.HasValue && PayerId.Value != Guid.Empty)
                {
                    transactions = this._FinanceTransactionRepository.GetCreditNotes(PayerId.Value, FromDate, ToDate, PageIndex, PageSize, out TotalRowCount);
                }
                else
                {
                    transactions = this._FinanceTransactionRepository.GetCreditNotes(FromDate, ToDate, PageIndex, PageSize, out TotalRowCount);
                }
                return transactions.ProjectedToCollection<CreditNoteModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public IEnumerable<CreditNoteModel> GetCreditNotesForReceiptsByPayer(Guid PayerId, DateTime ToDate, int PageIndex, int PageSize, out int TotalRowCount)
        {
            try
            {
                TotalRowCount = 0;
                IEnumerable<CreditNote> creditNotes = this._FinanceTransactionRepository.GetCreditNotesForReceiptsByPayer(PayerId, ToDate.Date, PageIndex, PageSize, out TotalRowCount);
                return creditNotes.ProjectedToCollection<CreditNoteModel>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        #endregion
    }
}